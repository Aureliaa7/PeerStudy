using Fluxor;
using System.Threading.Tasks;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using System;
using PeerStudy.Core.Models.Courses;
using System.Collections.Generic;
using Blazored.Toast.Services;

namespace PeerStudy.Features.Courses.Store
{
    public class CoursesEffects
    {
        private readonly ICourseService courseService;
        private readonly IToastService toastService;

        public CoursesEffects(
            ICourseService courseService,
            IToastService toastService)
        {
            this.courseService = courseService;
            this.toastService = toastService;
        }

        [EffectMethod]
        public async Task HandleAddCourseAction(AddCourseAction action, IDispatcher dispatcher)
        {
            try
            {
                var savedCourse = await courseService.AddAsync(action.Course);
                dispatcher.Dispatch(new AddActiveCourseAction(savedCourse));
                toastService.ShowSuccess("The course was saved");
            }
            catch (Exception ex)
            {
                toastService.ShowError("The course could not be saved...");
            }
        }

        [EffectMethod]
        public async Task HandleUpdateCourseAction(UpdateCourseAction action, IDispatcher dispatcher)
        {
            try
            {
                var savedCourse = await courseService.UpdateAsync(action.Course);
                dispatcher.Dispatch(new UpdateCourseSuccessAction(savedCourse));
            }
            catch (Exception ex)
            {
                toastService.ShowError("The course could not be updated..");
            }
        }


        [EffectMethod]
        public async Task HandleArchiveCourseAction(ArchiveCourseAction action, IDispatcher dispatcher)
        {
            try
            {
                await courseService.ArchiveCourseAsync(action.TeacherId, action.CourseId);
                dispatcher.Dispatch(new ArchiveCourseSuccessAction(action.CourseId));
            }
            catch (Exception ex)
            {
                toastService.ShowError("The course could not be archived...");
            }
        }

        [EffectMethod]
        public async Task HandleFetchArchivedCoursesAction(FetchArchivedCoursesAction action, IDispatcher dispatcher)
        {
            try
            {
                if (action.Role == Role.Teacher)
                {
                    var archivedCourses = await courseService.GetAsync(action.UserId, CourseStatus.Archived);

                    dispatcher.Dispatch(new FetchArchivedCoursesSuccessAction(archivedCourses));
                }
                else if (action.Role == Role.Student)
                {
                    var archivedCourses = await courseService.GetCoursesForStudentAsync(action.UserId, CourseStatus.Archived);

                    dispatcher.Dispatch(new FetchArchivedCoursesSuccessAction(archivedCourses));
                }
                else
                {
                    dispatcher.Dispatch(new FetchArchivedCoursesSuccessAction(new List<CourseDetailsModel>()));
                }
            }
            catch (Exception ex)
            {
                HandleLoadCoursesError(dispatcher);
            }
        }

        [EffectMethod]
        public async Task HandleFetchActiveCoursesAction(FetchActiveCoursesAction action, IDispatcher dispatcher)
        {
            try
            {
                if (action.Role == Role.Teacher)
                {
                    var archivedCourses = await courseService.GetAsync(action.UserId, CourseStatus.Active);

                    dispatcher.Dispatch(new FetchActiveCoursesSuccessAction(archivedCourses));
                }
                else if (action.Role == Role.Student)
                {
                    var archivedCourses = await courseService.GetCoursesForStudentAsync(action.UserId, CourseStatus.Active);

                    dispatcher.Dispatch(new FetchActiveCoursesSuccessAction(archivedCourses));
                }
                else
                {
                    dispatcher.Dispatch(new FetchActiveCoursesSuccessAction(new List<CourseDetailsModel>()));
                }
            }
            catch (Exception ex)
            {
                HandleLoadCoursesError(dispatcher);
            }
        }

        private void HandleLoadCoursesError(IDispatcher dispatcher)
        {
            toastService.ShowError("The courses could not be fetched...");
            dispatcher.Dispatch(new FetchCoursesErrorAction());
        }
    }
}

