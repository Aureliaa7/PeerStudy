using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Resources.Components.ResourcesListComponent
{
    public partial class ResourcesList
    {
        [Parameter]
        public List<ResourceDetailsModel> Resources { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteResource { get; set; }

        private const string coursesNotFoundMessage = "There are no resources yet...";

        private async Task HandleDeleteResource(Guid resourceId)
        {
            await OnDeleteResource.InvokeAsync(resourceId);
        }
    }
}
