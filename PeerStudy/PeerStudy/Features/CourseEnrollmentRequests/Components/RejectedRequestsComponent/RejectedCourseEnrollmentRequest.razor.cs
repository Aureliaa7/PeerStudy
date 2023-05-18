using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.CourseEnrollments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseEnrollmentRequests.Components.RejectedRequestsComponent
{
    public partial class RejectedCourseEnrollmentRequest : CourseEnrollmentRequestBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task<List<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync()
        {
            return (await CourseEnrollmentService.GetRequestsAsync(TeacherId, CourseId, CourseEnrollmentRequestStatus.Rejected))
                .ToList();
        }
    }
}
