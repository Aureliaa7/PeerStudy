using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System.Threading.Tasks;
using System;

namespace PeerStudy.Features.Resources.Components
{
    public class ResourceBase : ComponentBase
    {
        [Parameter]
        public ResourceDetailsModel ResourceDetails { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteResource { get; set; }

        protected async Task HandleDeleteResource()
        {
            await OnDeleteResource.InvokeAsync(ResourceDetails.Id);
        }
    }
}
