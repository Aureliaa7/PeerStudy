using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CreateCourseDialog
    {
        [Parameter]
        public EventCallback<CreateCourseModel> OnCourseAdded { get; set; }

        [Parameter]
        public EventCallback<bool> OnCancel { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        private CreateCourseModel createCourseModel = new CreateCourseModel();

        private string datePickerStyleRules = "width: 80%;";

        private async Task SaveCourse()
        {
            await OnCourseAdded.InvokeAsync(createCourseModel);
        }

        private async Task Cancel()
        {
            IsVisible = false;
            await OnCancel.InvokeAsync();
        }
    }
}
