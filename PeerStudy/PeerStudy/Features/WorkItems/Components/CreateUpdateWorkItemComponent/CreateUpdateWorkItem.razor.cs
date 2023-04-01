using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.WorkItems;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.WorkItems.Components.CreateUpdateWorkItemComponent
{
    public partial class CreateUpdateWorkItem
    {
        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback<bool> OnCancel { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public CreateUpdateWorkItemModel WorkItemModel { get; set; }

        [Parameter]
        public List<DropDownItem> Students { get; set; } 

        [Parameter]
        public List<WorkItemStatus> WorkItemStatuses { get; set; }

        [Parameter]
        public DropDownItem? SelectedStudent { get; set; }

        private const string matFieldStyles = "width: 98%;";
        private const string descriptionFieldStyles = "width: 98%; height: 100px;";

        private async Task Save()
        {
            WorkItemModel.AssignedTo = SelectedStudent?.Key == null ? null : new Guid(SelectedStudent.Key);
            await OnSave.InvokeAsync();
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }
    }
}

