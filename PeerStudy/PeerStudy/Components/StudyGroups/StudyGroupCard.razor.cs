using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;

namespace PeerStudy.Components.StudyGroups
{
    public partial class StudyGroupCard
    {
        [Parameter]
        public StudyGroupDetailsModel StudyGroupModel { get; set; } 

        private const string styles = "width: 80%; height: 90%;";
    }
}
