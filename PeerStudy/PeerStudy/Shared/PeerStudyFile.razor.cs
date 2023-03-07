using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Shared
{
    public partial class PeerStudyFile
    {
        [Parameter]
        public string IconLink { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string WebViewLink { get; set; }

        [Parameter]
        public bool DisplayDeleteButton { get; set; }

        [Parameter]
        public EventCallback<string> OnDelete { get; set; }

        private async Task HandleDelete()
        {
            await OnDelete.InvokeAsync(Title);
        }
    }
}
