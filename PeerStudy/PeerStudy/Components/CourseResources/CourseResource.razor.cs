using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Components.CourseResources
{
    public partial class CourseResource
    {
        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public CourseResourceDetailsModel ResourceDetails { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteResource { get; set; }   

        private async Task HandleDeleteResource()
        {
            await OnDeleteResource.InvokeAsync(ResourceDetails.Id);
        }
    }
}
