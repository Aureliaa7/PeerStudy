using Fluxor;
using PeerStudy.Core.Enums;
using System.Linq;

namespace PeerStudy.Features.Courses.Store
{
    public class CoursesReducers
    {
        [ReducerMethod]
        public CoursesState ReduceFetchArchivedCoursesAction(CoursesState state, FetchArchivedCoursesAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }


        [ReducerMethod]
        public CoursesState ReduceFetchArchivedCoursesSuccessAction(CoursesState state, FetchArchivedCoursesSuccessAction action)
        {
            return state with
            {
                ArchivedCourses = action.ArchivedCourses,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public CoursesState ReduceFetchCoursesActionAction(CoursesState state, FetchCoursesErrorAction action)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public CoursesState ReduceFetchActiveCoursesAction(CoursesState state, FetchActiveCoursesAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }


        [ReducerMethod]
        public CoursesState ReduceFetchActiveCoursesSuccessAction(CoursesState state, FetchActiveCoursesSuccessAction action)
        {
            return state with
            {
                ActiveCourses = action.ActiveCourses,
                IsLoading = false
            };
        }

        [ReducerMethod]
        public CoursesState ReduceAddActiveCourseAction(CoursesState state, AddActiveCourseAction action)
        {
            var activeCourses = state.ActiveCourses;
            activeCourses.Add(action.ActiveCourse);

            return state with
            {
                ActiveCourses = activeCourses
            };
        }

        [ReducerMethod]
        public CoursesState ReduceUpdateCourseAction(CoursesState state, UpdateCourseSuccessAction action)
        {
            var activeCourses = state.ActiveCourses;

            var courseToBeUpdated = activeCourses.First(x => x.Id == action.UpdatedCourse.Id);
            courseToBeUpdated.Title = action.UpdatedCourse.Title;
            courseToBeUpdated.NoMaxStudents = action.UpdatedCourse.NoMaxStudents;

            return state with
            {
                ActiveCourses = activeCourses
            };
        }

        [ReducerMethod]
        public CoursesState ReduceArchiveCourseAction(CoursesState state, ArchiveCourseSuccessAction action)
        {
            var archivedCourse = state.ActiveCourses.First(x => x.Id == action.CourseId);
            archivedCourse.Status = CourseStatus.Archived;

            var activeCourses = state.ActiveCourses;
            var archivedCourses = state.ArchivedCourses;
            activeCourses.Remove(archivedCourse);
            archivedCourses.Add(archivedCourse);

            return state with
            {
                ActiveCourses = activeCourses,
                ArchivedCourses = archivedCourses
            };
        }
    }
}
