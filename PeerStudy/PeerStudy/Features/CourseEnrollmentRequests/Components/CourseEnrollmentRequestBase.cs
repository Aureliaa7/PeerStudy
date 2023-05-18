using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.CourseEnrollments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseEnrollmentRequests.Components
{
    public abstract class CourseEnrollmentRequestBase : PeerStudyComponentBase
    {
        [Inject]
        protected ICourseEnrollmentService CourseEnrollmentService { get; set; } 


        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }


        protected List<CourseEnrollmentRequestDetailsModel> requests;

        protected abstract Task<List<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                requests = await GetRequestsAsync();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.FetchRequestsFailedMessage);
            }
        }
    }
}
