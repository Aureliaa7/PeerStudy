using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.CourseEnrollments;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseEnrollmentRequests.Components.CourseEnrollmentRequestComponent
{
    public partial class CourseEnrollmentRequestList
    {
        [Parameter]
        public List<CourseEnrollmentRequestDetailsModel> Requests { get; set; }

        [Parameter]
        public bool DisplayActionButtons { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public bool CanDeleteRequests { get; set; }

        [Parameter]
        public string NoRequestsMessage { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        [Parameter]
        public CourseNoStudentsDetailsModel EnrolledStudentsStatus { get; set; }

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

        [Parameter]
        public EventCallback<List<CourseEnrollmentRequestDetailsModel>> OnDeleteRequests { get; set; }


        private List<CourseEnrollmentRequestDetailsModel> selectedRequests;
        private List<CourseEnrollmentRequestDetailsModel> currentRequests;
        private CourseEnrollmentRequestDetailsModel selectedRequest;
        private bool allowSelection;
        private DataGridSelectionMode selectionMode;
        private int noTotalPages;
        private int currentPageNumber;

        private const int pageSize = 10;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            noTotalPages = Convert.ToInt32(Math.Ceiling((double)Requests.Count / pageSize));
            currentPageNumber = 1;
            SetDataForPageNumber(currentPageNumber);
        }

        private async Task ApproveRequests()
        {
            //send the list of requests bc. the selectedRequest is always null even if there is only one item selected
            UpdateRequestsList(selectedRequests);
            await OnApproveRequests.InvokeAsync(selectedRequests);
        }

        private async Task RejectRequests()
        {
            UpdateRequestsList(selectedRequests);
            await OnRejectRequests.InvokeAsync(selectedRequests);
        }

        private async Task DeleteRequests()
        {
            UpdateRequestsList(selectedRequests);
            await OnDeleteRequests.InvokeAsync(selectedRequests);
        }

        private void UpdateRequestsList(List<CourseEnrollmentRequestDetailsModel> selectedRequests)
        {
            currentRequests = currentRequests.Except(selectedRequests).ToList();
            selectedRequests = null;
            StateHasChanged();
        }

        private bool AreButtonsDisabled()
        {
            return selectedRequests == null || !selectedRequests.Any();
        }

        private bool IsApproveButtonDisabled()
        {
            return selectedRequests?.Count() > EnrolledStudentsStatus?.NoMaxStudents - EnrolledStudentsStatus?.NoEnrolledStudents;
        }

        private void SetDataForPageNumber(int pageNumber)
        {
            currentPageNumber = pageNumber;
            currentRequests = Requests.Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .ToList();

            selectedRequests = null;
        }
    }
}
