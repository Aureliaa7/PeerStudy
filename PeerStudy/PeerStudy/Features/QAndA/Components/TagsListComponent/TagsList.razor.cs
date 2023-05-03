using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.TagsListComponent
{
    public partial class TagsList
    {
        [Parameter]
        public List<string> Tags { get; set; } = new List<string>();

        [Parameter]
        public bool ShowDeleteButton { get; set; }

        [Parameter]
        public EventCallback<string> OnDelete { get; set; }


        private async Task DeleteHandler(string tag)
        {
            await OnDelete.InvokeAsync(tag);
        }
    }
}
