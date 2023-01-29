using PeerStudy.Core.Models.Courses;
using System.Threading.Tasks;

namespace PeerStudy.Components.Courses
{
    public partial class CoursesList
    {

        private bool displayCreateCourseDialog = false;

        private string addCourseBtnStyle = "position: fixed; right: 30px; margin-bottom: 15px";

        public void ShowAddCourseDialog()
        {
            displayCreateCourseDialog = true;
        }

        public async Task SaveCourse(CreateCourseModel courseData)
        {
            bool isValidData = ModelValidator.IsModelValid<CreateCourseModel>(courseData);
            if (isValidData)
            {
                displayCreateCourseDialog = false;
                //TODO: save data to DB
            }
            await Task.Delay(0);
        }

        public void HideAddCourseDialog()
        {
            displayCreateCourseDialog = false;
        }
    }
}
