using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.StudyGroups;
using System.Collections.Generic;

namespace PeerStudy.Components.StudyGroups
{
    public partial class StudyGroupsList
    {
        [Parameter]
        public List<StudyGroupDetailsModel> StudyGroups { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }
    }
}
