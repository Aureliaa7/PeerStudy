using PeerStudy.Core.Models.QAndA.Answers;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IAnswerService
    {
        Task<AnswerDetailsModel> AddAsync(AddAnswerModel answerModel);

        Task DeleteAsync(Guid id, Guid authorId);

        Task UpdateAsync(UpdateAnswerModel updateAnswerModel);
    }
}
