using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.CourseEnrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseEnrollmentRequests.Components.StudentEnrollmentRequestsComponent
{
    public partial class StudentEnrollmentRequests : PeerStudyComponentBase
    {
        [Inject]
        private ICourseEnrollmentService CourseEnrollmentService { get; set; }

        private List<CourseEnrollmentRequestDetailsModel> requests;
        private List<CourseEnrollmentRequestStatus?> enrollmentStatuses = Enum.GetValues(typeof(CourseEnrollmentRequestStatus)).Cast<CourseEnrollmentRequestStatus?>().ToList();
        private CourseEnrollmentRequestStatus? enrollmentStatus = CourseEnrollmentRequestStatus.Pending;

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            requests = await CourseEnrollmentService.GetByStudentAndStatusAsync(currentUserId, enrollmentStatus.Value);
        }

        private async Task HandleSelectedStatusChanged(CourseEnrollmentRequestStatus? newStatus)
        {
            isLoading = true;
            enrollmentStatus = newStatus;
            requests.Clear();
            StateHasChanged();

            try
            {
                requests = await CourseEnrollmentService.GetByStudentAndStatusAsync(currentUserId, newStatus.Value);
            }
            catch
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while fetching the enrollment requests...");
            }
            isLoading = false;
        }

        private async Task DeleteRequests(List<CourseEnrollmentRequestDetailsModel> requests)
        {
            try
            {
                await CourseEnrollmentService.DeleteAsync(currentUserId, requests.Select(x => x.Id).ToList());
                requests = requests.Except(requests).ToList();
            }
            catch
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.DeleteCourseEnrollmentRequestsErrorMessage);
            }
        }
    }
}
