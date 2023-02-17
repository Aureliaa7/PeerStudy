using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.Students
{
    public partial class StudentsList : PeerStudyComponentBase<EnrolledStudentModel>
    {
        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        [Inject]
        public ICourseService CourseService { get; set; }

        private const string noEnrolledStudentsMessage = "There are no students enrolled yet...";

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override Task<List<EnrolledStudentModel>> GetDataAsync()
        {
            return CourseService.GetStudentsAsync(CourseId);
        }
    }
}
