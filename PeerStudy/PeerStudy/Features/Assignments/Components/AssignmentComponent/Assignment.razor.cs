using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.AssignmentComponent
{
    public partial class Assignment
    {
        [Parameter]
        public AssignmentDetailsModel Data { get; set; }

        [Parameter]
        public EventCallback<AssignmentDetailsModel> OnClick { get; set; }

        private async Task HandleClickedAssignment()
        {
            await OnClick.InvokeAsync(Data);
        }
    }
}
