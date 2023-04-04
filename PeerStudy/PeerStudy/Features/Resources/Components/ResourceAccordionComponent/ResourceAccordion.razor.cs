using Microsoft.AspNetCore.Components;

namespace PeerStudy.Features.Resources.Components.ResourceAccordionComponent
{
    public partial class ResourceAccordion : ResourceBase
    {
        [Parameter]
        public bool Expanded { get; set; }
    }
}
