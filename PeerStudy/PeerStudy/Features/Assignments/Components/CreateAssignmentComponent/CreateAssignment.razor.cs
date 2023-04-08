using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.CreateAssignmentComponent
{
    public partial class CreateAssignment
    {
        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public CreateAssignmentModel AssignmentModel { get; set; } = new();

        [Parameter]
        public List<DropDownItem> StudyGroups { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        private DropDownItem selectedStudyGroup;

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }

        private async Task SaveAssignment()
        {
            AssignmentModel.StudyGroupId = new Guid(selectedStudyGroup.Key);
            await OnConfirm.InvokeAsync();
        }
    }
}
