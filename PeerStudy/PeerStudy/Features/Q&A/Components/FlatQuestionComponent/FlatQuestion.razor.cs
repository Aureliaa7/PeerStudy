using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.QAndA.Questions;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.FlatQuestionComponent
{
    public partial class FlatQuestion
    {
        [Parameter]
        public FlatQuestionModel Question { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClick { get; set; }

        private async Task ClickHandler()
        {
            await OnClick.InvokeAsync(Question.Id);
        }
    }
}
