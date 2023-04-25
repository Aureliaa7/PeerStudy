using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public abstract class VotingBaseService
    {
        /// <summary>
        /// Deletes an existing vote
        /// </summary>
        /// <param name="entityId">The question/answer id</param>
        /// <param name="userId">The user id</param>
        /// <param name="voteType">The type of the vote</param>
        /// <returns>True if the vote was found and deleted</returns>
        protected abstract Task<bool> DeleteVoteIfExistsAsync(Guid entityId, Guid userId, VoteType voteType);

        protected abstract Task SaveVoteAsync(VoteModel voteModel);

        protected async Task VoteEntityAsync(VoteModel voteModel)
        {
            // if answer is upvoted/downvoted => delete the vote
            bool voteExisted = await DeleteVoteIfExistsAsync(voteModel.EntityId, voteModel.UserId, voteModel.VoteType);

            if (!voteExisted)
            {
                // if answer is downvoted => upvote it
                if (voteModel.VoteType == VoteType.Upvote)
                {
                    await DeleteVoteIfExistsAsync(voteModel.EntityId, voteModel.UserId, VoteType.Downvote);
                }
                // if answer is upvoted => downvote it
                else if (voteModel.VoteType == VoteType.Downvote)
                {
                    await DeleteVoteIfExistsAsync(voteModel.EntityId, voteModel.UserId, VoteType.Upvote);
                }

                await SaveVoteAsync(voteModel);
            }
        }
    }
}
