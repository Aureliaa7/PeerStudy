using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Emails;
using PeerStudy.Core.Models.ProgressModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class RewardingService : IRewardingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IStudentBadgeService studentBadgeService;
        private readonly IBadgeService badgeService;
        private readonly IEmailService emailService;

        private const int numberOfCourseBadges = 3;
        private const int firstPlace = 1;
        private const int secondPlace = 2;
        private const int thirdPlace = 3;

        public RewardingService(
            IUnitOfWork unitOfWork,
            IStudentBadgeService studentBadgeService,
            IBadgeService badgeService,
            IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.studentBadgeService = studentBadgeService;
            this.badgeService = badgeService;
            this.emailService = emailService;
        }

        public async Task UpdateBadgesForAnswersAsync(Guid studentId)
        {
            var postedAnswers = (await unitOfWork.AnswersRepository.GetAllAsync(x => x.AuthorId == studentId)).Count();

            Badge badge = null;

            if (postedAnswers == Constants.FirstPostedAnswer)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.FirstAnswer, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.FirstAnswer);
            }
            else if (postedAnswers == Constants.AnswersBronzeThreshold)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.AnswerContributor, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.AnswerContributor);
            }
            else if (postedAnswers % Constants.AnswersSilverThreshold == 0)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.Mentor, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.Mentor);
            }

            await NotifyStudentAboutEarnedBadgeAsync(studentId, badge);
        }

        public async Task UpdateBadgesForQuestionsAsync(Guid studentId)
        {
            var postedQuestions = (await unitOfWork.QuestionsRepository.GetAllAsync(x => x.AuthorId == studentId)).Count();

            Badge badge = null;

            if (postedQuestions == Constants.FirstPostedQuestion)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.FirstQuestion, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.FirstQuestion);
            }
            else if (postedQuestions == Constants.QuestionsBronzeThreshold)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.QuestionContributor, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.QuestionContributor);
            }
            else if (postedQuestions % Constants.QuestionsSilverThreshold == 0)
            {
                await studentBadgeService.AddAsync(studentId, BadgeType.QuestionMaster, StudentBadgeType.QAndA);
                badge = await badgeService.GetByTypeAsync(BadgeType.QuestionMaster);
            }

            await NotifyStudentAboutEarnedBadgeAsync(studentId, badge);
        }

        private async Task NotifyStudentAboutEarnedBadgeAsync(Guid studentId, Badge badge)
        {
            try
            {
                if (badge == null)
                {
                    throw new ArgumentNullException(nameof(badge));
                }

                var student = await unitOfWork.UsersRepository.GetFirstOrDefaultAsync(x => x.Id == studentId && x.Role == Role.Student)
                    ?? throw new EntityNotFoundException($"Student with id {studentId} was not found!");
            
                var emailModel = new EarnedBadgeEmailModel
                {
                    BadgeDescription = badge.Description,
                    BadgeTitle = badge.Title,
                    NoEarnedPoints = badge.Points,
                    RecipientName = $"{student.FirstName} {student.LastName}",
                    EmailType = EmailType.EarnedQAndABadge,
                    To = new List<string> { student.Email }
                };
                await emailService.SendAsync(emailModel);
            }
            catch { }
        }

        public async Task UpdateBadgesForUpvotedAnswerAsync(Guid answerId)
        {
            var answer = await unitOfWork.AnswersRepository.GetFirstOrDefaultAsync(x => x.Id == answerId) 
                ?? throw new EntityNotFoundException($"Answer with id {answerId} was not found!");

            var noUpvotes = (await unitOfWork.AnswerVotesRepository.GetAllAsync(
                x => x.AnswerId == answerId && x.VoteType == VoteType.Upvote)).Count();

            if (noUpvotes == Constants.FirstUpvotedAnswer)
            {
                await studentBadgeService.AddAsync(answer.AuthorId, BadgeType.FirstUpvotedAnswer, StudentBadgeType.QAndA);
            }
        }

        public async Task UpdateBadgesForUpvotedQuestionAsync(Guid questionId)
        {
            var question = await unitOfWork.QuestionsRepository.GetFirstOrDefaultAsync(x => x.Id == questionId)
               ?? throw new EntityNotFoundException($"Question with id {questionId} was not found!");

            var noUpvotes = (await unitOfWork.QuestionVotesRepository.GetAllAsync(
                x => x.QuestionId == questionId && x.VoteType == VoteType.Upvote)).Count();

            if (noUpvotes == Constants.FirstUpvotedQuestion)
            {
                await studentBadgeService.AddAsync(question.AuthorId, BadgeType.FirstUpvotedQuestion, StudentBadgeType.QAndA);
            }
        }

        public async Task UpdateBadgesForCourseAsync(Guid courseId)
        {
            var topStudents = await GetTopStudents(courseId);
            
            var earnedCourseBadges = (await unitOfWork.StudentBadgesRepository.GetAllAsync(x => x.Type == StudentBadgeType.Course
                && x.CourseId == courseId, includeProperties: nameof(Badge)))
            .ToList();

            await UpdateBadgesAsync(courseId, topStudents, earnedCourseBadges);  
        }

        private async Task UpdateBadgesAsync(Guid courseId, List<StudentCourseRankModel> topStudents, List<StudentBadge> badges)
        {
            await UpdateBadgeByTypeAsync(
                courseId,
                BadgeType.FirstPlaceInCourseTop,
                topStudents.Where(x => x.Rank == firstPlace).ToList(),
                badges);

            await UpdateBadgeByTypeAsync(
                courseId,
                BadgeType.SecondPlaceInCourseTop,
                topStudents.Where(x => x.Rank == secondPlace).ToList(),
                badges);

            await UpdateBadgeByTypeAsync(
                courseId,
                BadgeType.ThirdPlaceInCourseTop,
                topStudents.Where(x => x.Rank == thirdPlace).ToList(),
                badges);
        }

        private async Task UpdateBadgeByTypeAsync(Guid courseId, 
            BadgeType badgeType, 
            List<StudentCourseRankModel> students, 
            List<StudentBadge> earnedBadges)
        {
            var filteredBadges = GetBadgeByType(earnedBadges, badgeType);

            if (!filteredBadges.Any())
            {
                foreach (var student in students)
                {
                    await studentBadgeService.AddAsync(student.StudentId, badgeType, StudentBadgeType.Course, courseId);
                }
            }
            else
            {
                await DeleteRedundantBadgesAsync(filteredBadges, students.Select(x => x.StudentId).ToList());

                foreach (var student in students)
                {
                    bool hasBadge = filteredBadges.Any(x => x.StudentId == student.StudentId);
                    if (!hasBadge)
                    {
                        await studentBadgeService.AddAsync(student.StudentId, badgeType, StudentBadgeType.Course, courseId);
                    }
                }
            }
        }

        private async Task DeleteRedundantBadgesAsync(List<StudentBadge> earnedBadges, List<Guid> newStudentsIds)
        {
            var existingWinnersIds = earnedBadges
                .Select(x => x.StudentId)
                .ToList();

            var redundantStudentsIds = existingWinnersIds
                .Except(newStudentsIds)
                .ToList();

            var studentBadgesIdsToBeDeleted = earnedBadges
                .Where(x => redundantStudentsIds.Contains(x.StudentId))
                .Select(x => x.Id)
                .ToList();

            foreach (var id in studentBadgesIdsToBeDeleted)
            {
                await unitOfWork.StudentBadgesRepository.RemoveAsync(id);
            }
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<List<StudentCourseRankModel>> GetTopStudents(Guid courseId)
        {
            var enrolledStudents = await unitOfWork.StudentCourseRepository.GetAllAsync(x => x.CourseId == courseId);
            var studentAssignments = await unitOfWork.StudentAssignmentsRepository.GetAllAsync(x => x.Assignment.CourseUnit.CourseId == courseId);

            var studentCoursePoints = (from es in enrolledStudents
                                       join sa in studentAssignments on es.StudentId equals sa.StudentId
                                       group sa by sa.StudentId into g
                                       select new StudentCourseRankModel
                                       {
                                           StudentId = g.Key,
                                           Points = g.Sum(sa => sa.Points ?? 0)
                                       })
                                      .OrderByDescending(x => x.Points)
                                      .ToList();

            List<int> topRanks = studentCoursePoints
               .Select(x => x.Points)
               .Distinct()
               .Take(numberOfCourseBadges)
               .ToList();

            foreach (var student in studentCoursePoints)
            {
                int rank = topRanks.IndexOf(student.Points) + 1;
                if (rank <= numberOfCourseBadges)
                {
                    student.Rank = rank;
                }
                else
                {
                    break;
                }
            }

            return studentCoursePoints
                .Where(x => x.Rank > 0 && x.Rank <= numberOfCourseBadges)
                .ToList();
        }

        private static List<StudentBadge> GetBadgeByType(List<StudentBadge> earnedBadges, BadgeType badgeType)
        {
            return earnedBadges
                .Where(x => x.Badge.Type == badgeType)
                .ToList();
        }
    }
}
