using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;

namespace PeerStudy.Components.CourseResources
{
    public partial class CourseResources
    {
        [Parameter]
        public List<CourseResourceDetailsModel> Resources { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }
    }
}
