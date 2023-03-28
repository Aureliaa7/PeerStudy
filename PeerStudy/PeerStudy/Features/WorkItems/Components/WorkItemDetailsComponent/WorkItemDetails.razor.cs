using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.WorkItems;
using System.Threading.Tasks;

namespace PeerStudy.Features.WorkItems.Components.WorkItemDetailsComponent
{
    public partial class WorkItemDetails
    {
        [Parameter]
        public WorkItemDetailsModel WorkItem { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private async Task Close()
        {
            await OnClose.InvokeAsync();
        }
    }
}
