using Microsoft.AspNetCore.Components;

namespace PeerStudy.Shared
{
    public partial class Loading
    {
        [Parameter] 
        public bool IsLoading { get; set; }

        [Parameter]
        public bool HasData { get; set; }

        [Parameter]
        public string NotFoundMessage { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
