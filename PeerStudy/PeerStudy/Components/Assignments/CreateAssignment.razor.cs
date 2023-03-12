using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System.Threading.Tasks;

namespace PeerStudy.Components.Assignments
{
    public partial class CreateAssignment
    {
        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter]
        public CreateAssignmentModel AssignmentModel { get; set; } = new();

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }

        private async Task SaveAssignment()
        {
            await OnConfirm.InvokeAsync();
        }
    }
}
