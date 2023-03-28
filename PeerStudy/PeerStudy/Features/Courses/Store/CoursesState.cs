using Fluxor;
using PeerStudy.Core.Models.Courses;
using System.Collections.Generic;

namespace PeerStudy.Features.Courses.Store
{
    [FeatureState]
    public record CoursesState
    {
        public List<CourseDetailsModel> ActiveCourses { get; init; }

        public List<CourseDetailsModel> ArchivedCourses { get; init; }

        public bool IsLoading { get; init; }
    }
}
