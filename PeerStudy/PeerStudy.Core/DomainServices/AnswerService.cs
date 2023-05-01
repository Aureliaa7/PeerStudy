using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AnswerService : VotingBaseService, IAnswerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRewardingService rewardingService;

        public AnswerService(IUnitOfWork unitOfWork, IRewardingService rewardingService)
        {
            this.unitOfWork = unitOfWork;
            this.rewardingService = rewardingService;
        }

        public async Task<AnswerDetailsModel> AddAsync(AddAnswerModel answerModel)
        {
            bool questionExists = await unitOfWork.QuestionsRepository.ExistsAsync(x => x.Id == answerModel.QuestionId);
            if (!questionExists)
            {
                throw new EntityNotFoundException($"The question with id {answerModel.QuestionId} was not found!");
            }

            var savedAnswer = await unitOfWork.AnswersRepository.AddAsync(new Answer
            {
                AuthorId = answerModel.AuthorId,
                CreatedAt = DateTime.UtcNow,
                Content = answerModel.Content,
                QuestionId = answerModel.QuestionId
            });
            await unitOfWork.SaveChangesAsync();

            await rewardingService.UpdateBadgesForAnswersAsync(answerModel.AuthorId);

            return new AnswerDetailsModel
            {
                Id = savedAnswer.Id,
                CreatedAt = savedAnswer.CreatedAt,
                AuthorId = answerModel.AuthorId,
                HtmlContent = savedAnswer.Content
            };
        }

        public async Task DeleteAsync(Guid id, Guid authorId)
        {
            await CheckIfAnswerExistsAsync(id, authorId);

            await unitOfWork.AnswersRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateAnswerModel updateAnswerModel)
        {
            var answer = await unitOfWork.AnswersRepository.GetFirstOrDefaultAsync(x => x.Id == updateAnswerModel.Id &&
                x.AuthorId == updateAnswerModel.AuthorId)  
                ?? throw new EntityNotFoundException($"Answer with id {updateAnswerModel.Id} and authorId {updateAnswerModel.AuthorId} was not found!");
        
            answer.Content = updateAnswerModel.Content;
            await unitOfWork.AnswersRepository.UpdateAsync(answer);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task VoteAsync(VoteModel voteModel)
        {
            await VoteEntityAsync(voteModel);
            await rewardingService.UpdateBadgesForUpvotedAnswerAsync(voteModel.EntityId);
        }

        private async Task CheckIfAnswerExistsAsync(Guid id, Guid authorId)
        {
            bool answerExists = await unitOfWork.AnswersRepository.ExistsAsync(x => x.Id == id && x.AuthorId == authorId);

            if (!answerExists)
            {
                throw new EntityNotFoundException($"Answer with id {id} and authorId {authorId} was not found!");
            }
        }

        protected override async Task<bool> DeleteVoteIfExistsAsync(Guid answerId, Guid authorId, VoteType voteType)
        {
            var vote = await GetVoteAsync(answerId, authorId, voteType);

            if (vote != null)
            {
                await unitOfWork.AnswerVotesRepository.RemoveAsync(vote);
                await unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }

        protected override async Task SaveVoteAsync(VoteModel voteModel)
        {
            await unitOfWork.AnswerVotesRepository.AddAsync(new AnswerVote
            {
                AnswerId = voteModel.EntityId,
                AuthorId = voteModel.UserId,
                VoteType = voteModel.VoteType
            });
            await unitOfWork.SaveChangesAsync();
        }

        private Task<AnswerVote> GetVoteAsync(Guid answerId, Guid authorId, VoteType voteType)
        {
            return unitOfWork.AnswerVotesRepository.GetFirstOrDefaultAsync(
                x => x.AnswerId == answerId &&
                x.AuthorId == authorId &&
                x.VoteType == voteType);
        }
    }
}
