using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.QAndA.Answers;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.AnswerComponent
{
    public partial class AnswerDetails
    {
        [Parameter]
        public AnswerDetailsModel Answer { get; set; }

        [Parameter]
        public bool ShowActionButtons { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<(string, Guid)> OnSave { get; set; }

        [Parameter]
        public EventCallback<Guid> OnCancel { get; set; }


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
    }
}
