using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.CourseEnrollments;
using System;

namespace PeerStudy.Components.CourseEnrollmentRequests
{
    public abstract class CourseEnrollmentRequestBase : PeerStudyComponentBase<CourseEnrollmentRequestDetailsModel>
    {
        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        [Inject]
        public ICourseEnrollmentService CourseEnrollmentService { get; set; }
    }
}
