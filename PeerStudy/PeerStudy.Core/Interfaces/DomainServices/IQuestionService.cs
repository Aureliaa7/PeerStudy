using PeerStudy.Core.Models.QAndA;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IQuestionService
    {
        Task<QuestionDetailsModel> CreateAsync(CreateQuestionModel createQuestionModel);

        Task<QuestionDetailsModel> GetAsync(Guid id);

        Task<List<FlatQuestionModel>> GetFlatQuestionsAsync(Guid authorId);

        Task DeleteAsync(Guid id, Guid authorId);
    }
}
