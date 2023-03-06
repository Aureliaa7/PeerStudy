using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Components.Assignments
{
    public partial class Assignment
    {
        [Parameter]
        public AssignmentDetailsModel Data { get; set; }

        [Parameter]
        public EventCallback<Guid> OnClick { get; set; }

        private async Task HandleClickedAssignment()
        {
            await OnClick.InvokeAsync(Data.AssignmentId);
        }
    }
}
