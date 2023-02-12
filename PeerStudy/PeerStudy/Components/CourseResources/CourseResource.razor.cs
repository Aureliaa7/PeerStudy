using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;

namespace PeerStudy.Components.CourseResources
{
    public partial class CourseResource
    {
        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public CourseResourceDetailsModel ResourceDetails { get; set; }
    }
}
