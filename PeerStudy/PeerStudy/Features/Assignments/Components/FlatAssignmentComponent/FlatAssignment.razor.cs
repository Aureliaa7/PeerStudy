using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.FlatAssignmentComponent
{
    public partial class FlatAssignment
    {
        [Parameter]
        public FlatAssignmentModel Assignment { get; set; }

        [Parameter]
        public EventCallback<FlatAssignmentModel> OnClick { get; set; } 

        private async Task HandleClickedAssignment()
        {
            await OnClick.InvokeAsync(Assignment);
        }
    }
}
