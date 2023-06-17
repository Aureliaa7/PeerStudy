using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Emails;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AnswerService : VotingBaseService, IAnswerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRewardingService rewardingService;
        private readonly IEmailService emailService;

        public AnswerService(
            IUnitOfWork unitOfWork, 
            IRewardingService rewardingService,
            IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.rewardingService = rewardingService;
            this.emailService = emailService;
        }

        public async Task<AnswerDetailsModel> AddAsync(AddAnswerModel answerModel)
        {
            var question = await unitOfWork.QuestionsRepository.GetFirstOrDefaultAsync(x => x.Id == answerModel.QuestionId,
                includeProperties: nameof(Question.Author))
                ?? throw new EntityNotFoundException($"The question with id {answerModel.QuestionId} was not found!");
         
            var savedAnswer = await unitOfWork.AnswersRepository.AddAsync(new Answer
            {
                AuthorId = answerModel.AuthorId,
                CreatedAt = DateTime.UtcNow,
                Content = answerModel.Content,
                QuestionId = answerModel.QuestionId
            });
            await unitOfWork.SaveChangesAsync();

            if (await IsStudent(answerModel.AuthorId))
            {
                await rewardingService.UpdateBadgesForAnswersAsync(answerModel.AuthorId);
            }

            await NotifyNewQuestionResponseAsync(question, answerModel.AuthorId);

            return new AnswerDetailsModel
            {
                Id = savedAnswer.Id,
                CreatedAt = savedAnswer.CreatedAt,
                AuthorId = answerModel.AuthorId,
                HtmlContent = savedAnswer.Content
            };
        }

        private async Task NotifyNewQuestionResponseAsync(Question question, Guid responseAuthorId)
        {
            try
            {
                var answerAuthor = await unitOfWork.UsersRepository.GetByIdAsync(responseAuthorId);

                var emailModel = new NewQuestionAnswerEmailModel
                {
                    EmailType = EmailType.NewQuestionResponse,
                    QuestionTitle = question.Title,
                    AnswerAuthorName = $"{answerAuthor.FirstName} {answerAuthor.LastName}",
                    RecipientName = $"{question.Author.FirstName} {question.Author.LastName}",
                    To = new List<string> { question.Author.Email }
                };

                await emailService.SendAsync(emailModel);
            }
            catch { }
        }

        private async Task<bool> IsStudent(Guid userId)
        {
            var author = await unitOfWork.UsersRepository.GetByIdAsync(userId) ??
              throw new EntityNotFoundException($"User with id {userId} was not found!");

            return author.Role == Role.Student;
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
            var answer = await unitOfWork.AnswersRepository.GetByIdAsync(voteModel.EntityId)
                ?? throw new EntityNotFoundException($"Answer with id {voteModel.EntityId} was not found!");

            await VoteEntityAsync(voteModel);

            if (await IsStudent(answer.AuthorId))
            {
                await rewardingService.UpdateBadgesForUpvotedAnswerAsync(voteModel.EntityId);
            }
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
