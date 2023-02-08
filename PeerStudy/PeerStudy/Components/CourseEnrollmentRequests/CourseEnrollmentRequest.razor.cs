using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.CourseEnrollments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.CourseEnrollmentRequests
{
    public partial class CourseEnrollmentRequest
    {
        [Parameter]
        public List<CourseEnrollmentRequestDetailsModel> Requests { get; set; }

        [Parameter]
        public bool DisplayActionButtons { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public string NoRequestsMessage { get; set; }

        [Parameter]
        public bool AllowMultipleSelection
        {
            get 
            { 
                return allowSelection; 
            }
            set
            {
                allowSelection = value;
                selectionMode = allowSelection ? DataGridSelectionMode.Multiple : DataGridSelectionMode.Single;
            }
        }

        [Parameter]
        public EventCallback<List<CourseEnrollmentRequestDetailsModel>> OnApproveRequests { get; set; }

        [Parameter]
        public EventCallback<List<CourseEnrollmentRequestDetailsModel>> OnRejectRequests { get; set; }

        private List<CourseEnrollmentRequestDetailsModel> selectedRequests;
        private CourseEnrollmentRequestDetailsModel selectedRequest;

        private bool allowSelection;
        private DataGridSelectionMode selectionMode;


        private async Task ApproveRequests()
        {
            //send the list of requests bc. the selectedRequest is always null even if there is only one item selected
            await OnApproveRequests.InvokeAsync(selectedRequests);
            ResetSelectedRequests();
        }

        private async Task RejectRequests()
        {
            await OnRejectRequests.InvokeAsync(selectedRequests);
            ResetSelectedRequests();
        }

        private void ResetSelectedRequests()
        {
            selectedRequests = null;
            StateHasChanged();
        }

        private bool AreButtonsDisabled()
        {
            return selectedRequests == null || !selectedRequests.Any();
        }
    }
}
