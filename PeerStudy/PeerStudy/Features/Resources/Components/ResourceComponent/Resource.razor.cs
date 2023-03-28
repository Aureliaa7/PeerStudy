using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Resources.Components.ResourceComponent
{
    public partial class Resource
    {
        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public ResourceDetailsModel ResourceDetails { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteResource { get; set; }

        private async Task HandleDeleteResource()
        {
            await OnDeleteResource.InvokeAsync(ResourceDetails.Id);
        }
    }
}
