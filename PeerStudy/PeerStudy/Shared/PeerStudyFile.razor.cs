using Microsoft.AspNetCore.Components;

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
    }
}
