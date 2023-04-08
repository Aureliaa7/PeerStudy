using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PeerStudy.Features.Assignments.Components.StudentAssignmentsComponent
{
    public partial class StudentAssignments
    {
        [Parameter]
        public List<AssignmentDetailsModel> Assignments { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public EventCallback<AssignmentDetailsModel> OnClickedAssignment { get; set; }


        private bool expandedPanel;

        private async Task HandleClickedAssignment(AssignmentDetailsModel assignment)
        {
            await OnClickedAssignment.InvokeAsync(assignment);
        }
    }
}
