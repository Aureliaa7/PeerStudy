using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.TagComponent
{
    public partial class Tag
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public bool ShowDeleteButton { get; set; }

        [Parameter]
        public EventCallback<string> OnDelete { get; set; }

        private async Task DeleteHandler()
        {
            await OnDelete.InvokeAsync(Value);
        } 
    }
}
