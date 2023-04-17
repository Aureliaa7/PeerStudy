using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Models.QAndA.Questions;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IQuestionPaginationService : IPaginationService<FlatQuestionModel, Question>
    {
    }
}
