using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Courses;

namespace PeerStudy.Components.Courses
{
    public partial class CourseCard
    {
        [Parameter]
        public CourseDetailsModel Course { get; set; }

    }
}
