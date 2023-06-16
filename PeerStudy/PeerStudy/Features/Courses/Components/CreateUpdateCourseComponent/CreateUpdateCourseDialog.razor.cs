using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;
using System.Threading.Tasks;

namespace PeerStudy.Features.Courses.Components.CreateUpdateCourseComponent
{
    public partial class CreateUpdateCourseDialog
    {
        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public CourseModel CourseModel { get; set; }

        [Parameter]
        public bool IsEditCourseEnabled { get; set; }

        [Parameter]
        public bool HasStudyGroups { get; set; }

        [Parameter]
        public string DialogTitle { get; set; }


        private string datePickerStyleRules = "width: 80%;";
        private string dialogStyleRules = "top: -95px;";

        private async Task SaveCourse()
        {
            await OnSave.InvokeAsync();
        }

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }
    }
}
