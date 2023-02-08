using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.CourseEnrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.CourseEnrollmentRequests
{
    public partial class RejectedCourseEnrollmentRequest : PeerStudyComponentBase<CourseEnrollmentRequestDetailsModel>
    {
        [Inject]
        public ICourseEnrollmentService CourseEnrollmentService { get; set; }

        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        private const string noRequestsMessage = "There are no rejected requests...";

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override async Task<List<CourseEnrollmentRequestDetailsModel>> GetDataAsync()
        {
            return (await CourseEnrollmentService.GetRequestsAsync(TeacherId, CourseId, CourseEnrollmentRequestStatus.Rejected))
                .ToList();
        }  
    }
}
