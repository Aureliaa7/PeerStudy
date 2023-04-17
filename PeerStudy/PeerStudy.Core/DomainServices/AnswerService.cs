using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.QAndA.Answers;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class AnswerService : IAnswerService
    {
        private readonly IUnitOfWork unitOfWork;

        public AnswerService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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

        private async Task CheckIfAnswerExistsAsync(Guid id, Guid authorId)
        {
            bool answerExists = await unitOfWork.AnswersRepository.ExistsAsync(x => x.Id == id && x.AuthorId == authorId);

            if (!answerExists)
            {
                throw new EntityNotFoundException($"Answer with id {id} and authorId {authorId} was not found!");
            }
        }
    }
}
