using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components
{
    public partial class StudyGroupsListBase<T> : ComponentBase where T : StudyGroupDetailsModel
    {
        [Parameter]
        public List<T> StudyGroups { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<T> OnClicked { get; set; }

        protected async Task HandleClick(T studyGroup)
        {
            await OnClicked.InvokeAsync(studyGroup);
        }
    }
}
