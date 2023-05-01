using PeerStudy.Core.Models.QAndA.Votes;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IVotingService
    {
        Task VoteAsync(VoteModel voteModel);
    }
}
