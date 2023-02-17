using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.CourseEnrollments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.CourseEnrollmentRequests
{
    public partial class PendingCourseEnrollmentRequest : CourseEnrollmentRequestBase
    {
        private bool displayChangeRequestResult;
        private string changeRequestStatusResult;
        private const string noPendingRequestsMessage = "There are no pending requests...";

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override async Task<List<CourseEnrollmentRequestDetailsModel>> GetDataAsync()
        {
            return (await CourseEnrollmentService.GetRequestsAsync(TeacherId, CourseId, CourseEnrollmentRequestStatus.Pending))
                .ToList();
        }

        private async Task ApproveRequests(List<CourseEnrollmentRequestDetailsModel> requests)
        {
            await ChangeStatusAsync(requests, CourseEnrollmentRequestStatus.Approved);
        }

        private async Task RejectRequests(List<CourseEnrollmentRequestDetailsModel> requests)
        {
            await ChangeStatusAsync(requests, CourseEnrollmentRequestStatus.Rejected);
        }

        private async Task ChangeStatusAsync(List<CourseEnrollmentRequestDetailsModel> requests, CourseEnrollmentRequestStatus status)
        {
            bool result = await CourseEnrollmentService.ChangeStatusAsync(requests, status);

            if (result)
            {
                changeRequestStatusResult = "The requests were successfully updated";
                data = data.Except(requests).ToList();
            }
            else
            {
                changeRequestStatusResult = "An unexpected error occurred...";
            }
            displayChangeRequestResult = true;
        }
    }
}
