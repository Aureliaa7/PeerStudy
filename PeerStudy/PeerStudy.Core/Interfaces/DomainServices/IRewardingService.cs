using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IRewardingService
    {
        Task UpdateBadgesForQuestionsAsync(Guid authorId);

        Task UpdateBadgesForAnswersAsync(Guid authorId);

        Task UpdateBadgesForUpvotedAnswerAsync(Guid answerId);

        Task UpdateBadgesForUpvotedQuestionAsync(Guid questionId);
    }
}
