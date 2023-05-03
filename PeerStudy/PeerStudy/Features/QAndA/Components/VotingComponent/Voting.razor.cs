using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.VotingComponent
{
    public partial class Voting
    {
        [Parameter]
        public int NoUpvotes { get; set; }

        [Parameter]
        public int NoDownvotes { get; set; }

        [Parameter]
        public List<VoteDetailsModel> Votes { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback OnUpvote { get; set; }

        [Parameter]
        public EventCallback OnDownvote { get; set; }


        private async Task Upvote()
        {
            await OnUpvote.InvokeAsync();
        }

        private async Task Downvote()
        {
            await OnDownvote.InvokeAsync();
        }

        private bool CurrentUserVotedAnswer(VoteType voteType)
        {
            return Votes.Exists(x => x.UserId == CurrentUserId && x.VoteType == voteType);
        }
    }
}
