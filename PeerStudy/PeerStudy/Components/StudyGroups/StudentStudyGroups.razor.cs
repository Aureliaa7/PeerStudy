using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.StudyGroups
{
    public partial class StudentStudyGroups : PeerStudyComponentBase<StudyGroupDetailsModel>
    {
        [Inject]
        public IStudyGroupService StudyGroupService { get; set; }

        [Inject]
        private ICacheService CacheService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeDataAsync();
        }

        protected override Task<List<StudyGroupDetailsModel>> GetDataAsync()
        {
            return CacheService.GetAsync($"{currentUserId}_{ClientConstants.StudentStudyGroupsKey}",
                   () => StudyGroupService.GetByStudentIdAsync(currentUserId));
        }
    }
}
