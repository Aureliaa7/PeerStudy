﻿using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.Repositories;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<User> UsersRepository { get; }

        IRepository<Course> CoursesRepository { get; }

        IRepository<CourseEnrollmentRequest> CourseEnrollmentRequestsRepository { get; }

        IRepository<StudentCourse> StudentCourseRepository { get; }

        IRepository<CourseResource> CourseResourcesRepository { get; }

        IRepository<StudyGroup> StudyGroupRepository { get; }

        IRepository<StudentStudyGroup> StudentStudyGroupRepository { get; }

        IRepository<Assignment> AssignmentsRepository { get; }

        IRepository<StudentAssignment> StudentAssignmentsRepository { get; }

        IRepository<StudyGroupAssignmentFile> StudyGroupAssignmentFilesRepository { get; }

        IRepository<StudyGroupFile> StudyGroupFilesRepository { get; }

        IRepository<WorkItem> WorkItemsRepository { get; }

        IRepository<CourseUnit> CourseUnitsRepository { get; }

        IRepository<UnlockedCourseUnit> UnlockedCourseUnitsRepository { get; }

        IRepository<Question> QuestionsRepository { get; }

        IRepository<Tag> TagsRepository { get; }

        IRepository<QuestionTag> QuestionTagsRepository { get; }

        IRepository<Answer> AnswersRepository { get; }

        IRepository<AnswerVote> AnswerVotesRepository { get; }

        IRepository<QuestionVote> QuestionVotesRepository { get; }

        IRepository<Badge> BadgesRepository { get; }

        IRepository<StudentBadge> StudentBadgesRepository { get; }

        IRepository<PostponedAssignment> PostponedAssignmentsRepository { get; }

        IRepository<EmailTemplate> EmailTemplatesRepository { get; }

        Task SaveChangesAsync();
    }
}
