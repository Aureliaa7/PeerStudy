using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.StudyGroupCardComponent
{
    public partial class StudyGroupCard
    {
        [Parameter]
        public StudyGroupDetailsModel StudyGroupModel { get; set; } 

        [Parameter]
        public EventCallback<StudyGroupDetailsModel> OnClicked { get; set; }

        private const string styles = "width: 80%; height: 90%;";


        private async Task ClickHandler()
        {
            await OnClicked.InvokeAsync(StudyGroupModel);
        }
    }
}
