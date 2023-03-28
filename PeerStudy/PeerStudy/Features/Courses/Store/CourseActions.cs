using PeerStudy.Core.Enums;
using PeerStudy.Core.Models.Courses;
using System;
using System.Collections.Generic;

namespace PeerStudy.Features.Courses.Store
{
    public class AddActiveCourseAction
    {
        public CourseDetailsModel ActiveCourse { get; }

        public AddActiveCourseAction(CourseDetailsModel course)
        {
            ActiveCourse = course;
        }
    }

    public class AddCourseAction
    {
        public CourseModel Course { get; }

        public AddCourseAction(CourseModel createCourseModel)
        {
            Course = createCourseModel;
        }
    }

    public class UpdateCourseAction
    {
        public UpdateCourseModel Course { get; }

        public UpdateCourseAction(UpdateCourseModel course)
        {
            Course = course;
        }
    }

    public class UpdateCourseSuccessAction
    {
        public CourseDetailsModel UpdatedCourse { get; }

        public UpdateCourseSuccessAction(CourseDetailsModel course)
        {
            UpdatedCourse = course;
        }
    }

    public class ArchiveCourseAction
    {
        public Guid CourseId { get; }

        public Guid TeacherId { get; }

        public ArchiveCourseAction(Guid courseId, Guid teacherId)
        {
            CourseId = courseId;
            TeacherId = teacherId;
        }
    }

    public class ArchiveCourseSuccessAction
    {
        public Guid CourseId { get; }

        public ArchiveCourseSuccessAction(Guid courseId)
        {
            CourseId = courseId;
        }
    }

    public class FetchArchivedCoursesAction
    {
        public Guid UserId { get; }

        public Role? Role { get; }

        public FetchArchivedCoursesAction(Guid userId, Role? userRole)
        {
            UserId = userId;
            Role = userRole;
        }
    }

    public class FetchArchivedCoursesSuccessAction
    {
        public List<CourseDetailsModel> ArchivedCourses { get; }

        public FetchArchivedCoursesSuccessAction(List<CourseDetailsModel> courses)
        {
            ArchivedCourses = courses;
        }
    }

    public class FetchCoursesErrorAction { }

    public class FetchActiveCoursesAction
    {
        public Guid UserId { get; }

        public Role? Role { get; }

        public FetchActiveCoursesAction(Guid userId, Role? userRole)
        {
            UserId = userId;
            Role = userRole;
        }
    }

    public class FetchActiveCoursesSuccessAction
    {
        public List<CourseDetailsModel> ActiveCourses { get; }

        public FetchActiveCoursesSuccessAction(List<CourseDetailsModel> courses)
        {
            ActiveCourses = courses;
        }
    }
}
