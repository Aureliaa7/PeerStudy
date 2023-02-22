using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.StudyGroups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.StudyGroups
{
    public partial class CourseStudyGroups : PeerStudyComponentBase<StudyGroupDetailsModel>
    {
        [Parameter]
        public Guid CourseId { get; set; }

        [Parameter]
        public string CourseTitle { get; set; }

        [Inject]
        public IStudyGroupService StudyGroupService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override Task<List<StudyGroupDetailsModel>> GetDataAsync()
        {
            return StudyGroupService.GetByCourseIdAsync(CourseId);
        }
    }
}
