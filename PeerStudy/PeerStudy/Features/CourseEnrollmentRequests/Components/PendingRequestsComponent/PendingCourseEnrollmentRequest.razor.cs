using Blazored.Toast.Services;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.CourseEnrollments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseEnrollmentRequests.Components.PendingRequestsComponent
{
    public partial class PendingCourseEnrollmentRequest : CourseEnrollmentRequestBase
    {
        private const string noPendingRequestsMessage = "There are no pending requests...";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task<List<CourseEnrollmentRequestDetailsModel>> GetRequestsAsync()
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

        private async Task ChangeStatusAsync(List<CourseEnrollmentRequestDetailsModel> requestsToBeUpdated, CourseEnrollmentRequestStatus status)
        {
            bool result = await CourseEnrollmentService.ChangeStatusAsync(requestsToBeUpdated, status);

            if (result)
            {
                requests = requests.Except(requestsToBeUpdated).ToList();
                ToastService.ShowToast(ToastLevel.Success,"The requests were successfully updated");
            }
            else
            {
                ToastService.ShowToast(ToastLevel.Error, "An unexpected error occurred...");
            }
        }
    }
}
