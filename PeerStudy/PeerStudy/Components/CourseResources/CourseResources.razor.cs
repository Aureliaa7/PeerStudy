using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components.CourseResources
{
    public partial class CourseResources
    {
        [Parameter]
        public List<CourseResourceDetailsModel> Resources { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteResource { get; set; }

        private async Task HandleDeleteResource(Guid resourceId)
        {
            await OnDeleteResource.InvokeAsync(resourceId);
        }
    }
}
