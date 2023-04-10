using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Shared.ConfirmationPopupComponent
{
    public partial class ConfirmationPopup
    {
        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public bool IsConfirmButtonDisabled { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback OnConfirm  { get; set; }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        } 

        private async Task Confirm()
        {
            await OnConfirm.InvokeAsync();
        }
    }
}
