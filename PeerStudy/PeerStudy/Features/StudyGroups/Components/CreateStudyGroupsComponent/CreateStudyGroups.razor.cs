using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Features.StudyGroups.Components.CreateStudyGroupsComponent
{
    public partial class CreateStudyGroups
    {
        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public int[] DropDownItems { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<string> OnConfirm { get; set; }

        private string selectedValue { get; set; }

        private async Task Cancel()
        {
            selectedValue = string.Empty;
            await OnCancel.InvokeAsync();
        }

        private async Task CreateGroups()
        {
            await OnConfirm.InvokeAsync(selectedValue);
        }
    }
}
