using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.Repositories;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Infrastructure.AppDbContext;
using PeerStudy.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;
        private IRepository<User> usersRepository;
        private IRepository<Course> coursesRepository;
        private IRepository<CourseEnrollmentRequest> courseEnrollmentRequestsRepository;
        private IRepository<StudentCourse> studentCoursesRepository;
        private IRepository<CourseResource> courseResourcesRepository;
        private IRepository<StudyGroup> studyGroupRepository;
        private IRepository<StudentStudyGroup> studentStudyGroupRepository;
        private IRepository<Assignment> assignmentRepository;
        private IRepository<StudentAssignment> studentAssignmentRepository;
        private IRepository<StudentAssignmentFile> studentAssignmentFilesRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IRepository<User> UsersRepository
        {
            get
            {
                if (usersRepository == null)
                {
                    usersRepository = new Repository<User>(dbContext);
                }
                return usersRepository;
            }
        }

        public IRepository<Course> CoursesRepository
        {
            get
            {
                if (coursesRepository == null)
                {
                    coursesRepository = new Repository<Course>(dbContext);
                }
                return coursesRepository;
            }
        }

        public IRepository<CourseEnrollmentRequest> CourseEnrollmentRequestsRepository
        {
            get
            {
                if (courseEnrollmentRequestsRepository == null)
                {
                    courseEnrollmentRequestsRepository = new Repository<CourseEnrollmentRequest>(dbContext);
                }
                return courseEnrollmentRequestsRepository;
            }
        }
        public IRepository<StudentCourse> StudentCourseRepository
        {
            get
            {
                if (studentCoursesRepository == null)
                {
                    studentCoursesRepository = new Repository<StudentCourse>(dbContext);
                }
                return studentCoursesRepository;
            }
        }

        public IRepository<CourseResource> CourseResourcesRepository
        {
            get
            {
                if (courseResourcesRepository == null)
                {
                    courseResourcesRepository = new Repository<CourseResource>(dbContext);
                }
                return courseResourcesRepository;
            }
        }

        public IRepository<StudyGroup> StudyGroupRepository
        {
            get
            {
                if (studyGroupRepository == null)
                {
                    studyGroupRepository = new Repository<StudyGroup>(dbContext);
                }
                return studyGroupRepository;
            }
        }

        public IRepository<StudentStudyGroup> StudentStudyGroupRepository
        {
            get
            {
                if (studentStudyGroupRepository == null)
                {
                    studentStudyGroupRepository = new Repository<StudentStudyGroup>(dbContext);
                }
                return studentStudyGroupRepository;
            }
        }

        public IRepository<Assignment> AssignmentsRepository
        {
            get
            {
                if (assignmentRepository == null)
                {
                    assignmentRepository = new Repository<Assignment>(dbContext);
                }
                return assignmentRepository;
            }
        }

        public IRepository<StudentAssignment> StudentAssignmentsRepository
        {
            get
            {
                if (studentAssignmentRepository == null)
                {
                    studentAssignmentRepository = new Repository<StudentAssignment>(dbContext);
                }
                return studentAssignmentRepository;
            }
        }

        public IRepository<StudentAssignmentFile> StudentAssignmentFilesRepository
        {
            get
            {
                if (studentAssignmentFilesRepository == null)
                {
                    studentAssignmentFilesRepository = new Repository<StudentAssignmentFile>(dbContext);
                }
                return studentAssignmentFilesRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
