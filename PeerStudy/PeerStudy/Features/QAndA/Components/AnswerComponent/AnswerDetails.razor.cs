using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.QAndA.Answers;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.AnswerComponent
{
    public partial class AnswerDetails
    {
        [Parameter]
        public AnswerDetailsModel Answer { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<(string, Guid)> OnSave { get; set; }

        [Parameter]
        public EventCallback<Guid> OnCancel { get; set; }

        [Parameter]
        public EventCallback<Guid> OnUpvote { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDownvote { get; set; }

        private async Task Delete()
        {
            await OnDelete.InvokeAsync(Answer.Id);
        }

        private async Task Edit()
        {
            await OnEdit.InvokeAsync(Answer.Id);
        }

        private async Task SaveEditorContent(string htmlContent)
        {
            await OnSave.InvokeAsync((htmlContent, Answer.Id));
        }

        private async Task CancelEdit()
        {
            await OnCancel.InvokeAsync(Answer.Id);
        }

        private async Task Upvote()
        {
            await OnUpvote.InvokeAsync(Answer.Id);
        }

        private async Task Downvote()
        {
            await OnDownvote.InvokeAsync(Answer.Id);
        }
    }
}
