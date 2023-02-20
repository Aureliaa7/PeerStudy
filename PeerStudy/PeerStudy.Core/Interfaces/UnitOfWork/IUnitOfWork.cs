﻿using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.Repositories;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<User> UsersRepository { get; }

        IRepository<Course> CoursesRepository { get; }

        IRepository<CourseEnrollmentRequest> CourseEnrollmentRequestsRepository { get; }

        IRepository<StudentCourse> StudentCourseRepository { get; }

        IRepository<CourseResource> CourseResourcesRepository { get; }

        IRepository<StudyGroup> StudyGroupRepository { get; }

        Task SaveChangesAsync();
    }
}
