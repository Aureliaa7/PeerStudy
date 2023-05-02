using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class RewardingService : IRewardingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBadgeService badgeService;

        public RewardingService(IUnitOfWork unitOfWork, IBadgeService badgeService)
        {
            this.unitOfWork = unitOfWork;
            this.badgeService = badgeService;
        }

        public async Task UpdateBadgesForAnswersAsync(Guid studentId)
        {
            var postedAnswers = (await unitOfWork.AnswersRepository.GetAllAsync(x => x.AuthorId == studentId)).Count();

            if (postedAnswers == Constants.FirstPostedAnswer)
            {
                await badgeService.AddAsync(studentId, BadgeType.FirstAnswer);
            }
            else if (postedAnswers == Constants.AnswersBronzeThreshold)
            {
                await badgeService.AddAsync(studentId, BadgeType.AnswerContributor);
            }
            else if (postedAnswers % Constants.AnswersSilverThreshold == 0)
            {
                await badgeService.AddAsync(studentId, BadgeType.Mentor);
            }
        }

        public async Task UpdateBadgesForQuestionsAsync(Guid studentId)
        {
            var postedQuestions = (await unitOfWork.QuestionsRepository.GetAllAsync(x => x.AuthorId == studentId)).Count();

            if (postedQuestions == Constants.FirstPostedQuestion)
            {
                await badgeService.AddAsync(studentId, BadgeType.FirstQuestion);
            }
            else if (postedQuestions == Constants.QuestionsBronzeThreshold)
            {
                await badgeService.AddAsync(studentId, BadgeType.QuestionContributor);
            }
            else if (postedQuestions % Constants.QuestionsSilverThreshold == 0)
            {
                await badgeService.AddAsync(studentId, BadgeType.QuestionMaster);
            }
        }

        public async Task UpdateBadgesForUpvotedAnswerAsync(Guid answerId)
        {
            var answer = await unitOfWork.AnswersRepository.GetFirstOrDefaultAsync(x => x.Id == answerId) 
                ?? throw new EntityNotFoundException($"Answer with id {answerId} was not found!");

            var noUpvotes = (await unitOfWork.AnswerVotesRepository.GetAllAsync(
                x => x.AnswerId == answerId && x.VoteType == VoteType.Upvote)).Count();

            if (noUpvotes == Constants.FirstUpvotedAnswer)
            {
                await badgeService.AddAsync(answer.AuthorId, BadgeType.FirstUpvotedAnswer);
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
                await badgeService.AddAsync(question.AuthorId, BadgeType.FirstUpvotedQuestion);
            }
        }
    }
}
