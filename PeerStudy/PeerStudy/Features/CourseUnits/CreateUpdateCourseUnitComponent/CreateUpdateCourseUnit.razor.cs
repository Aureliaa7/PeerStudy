using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.CourseUnits;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseUnits.CreateUpdateCourseUnitComponent
{
    public partial class CreateUpdateCourseUnit
    {
        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public CourseUnitModel CourseUnitModel { get; set; }

        [Parameter]
        public bool IsEditModeEnabled { get; set; }


        private async Task Save()
        {
            await OnSave.InvokeAsync();
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }
    }
}
