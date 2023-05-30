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
        private IRepository<StudyGroupAssignmentFile> studentAssignmentFilesRepository;
        private IRepository<StudyGroupFile> studyGroupFilesRepository;
        private IRepository<WorkItem> workItemsRepository;
        private IRepository<CourseUnit> courseUnitsRepository;
        private IRepository<UnlockedCourseUnit> unlockedCourseUnitsRepository;
        private IRepository<Question> questionsRepository;
        private IRepository<Tag> tagsRepository;
        private IRepository<QuestionTag> questionTagsRepository;
        private IRepository<Answer> answersRepository;
        private IRepository<AnswerVote> answerVotesRepository;
        private IRepository<QuestionVote> questionVotesRepository;
        private IRepository<Badge> badgesRepository;
        private IRepository<StudentBadge> studentBadgesRepository;
        private IRepository<PostponedAssignment> postponedAssignmentsRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IRepository<User> UsersRepository
        {
            get
            {
                usersRepository ??= new Repository<User>(dbContext);
                return usersRepository;
            }
        }

        public IRepository<Course> CoursesRepository
        {
            get
            {
                coursesRepository ??= new Repository<Course>(dbContext);
                return coursesRepository;
            }
        }

        public IRepository<CourseEnrollmentRequest> CourseEnrollmentRequestsRepository
        {
            get
            {
                courseEnrollmentRequestsRepository ??= new Repository<CourseEnrollmentRequest>(dbContext);
                return courseEnrollmentRequestsRepository;
            }
        }
        public IRepository<StudentCourse> StudentCourseRepository
        {
            get
            {
                studentCoursesRepository ??= new Repository<StudentCourse>(dbContext);
                return studentCoursesRepository;
            }
        }

        public IRepository<CourseResource> CourseResourcesRepository
        {
            get
            {
                courseResourcesRepository ??= new Repository<CourseResource>(dbContext);
                return courseResourcesRepository;
            }
        }

        public IRepository<StudyGroup> StudyGroupRepository
        {
            get
            {
                studyGroupRepository ??= new Repository<StudyGroup>(dbContext);
                return studyGroupRepository;
            }
        }

        public IRepository<StudentStudyGroup> StudentStudyGroupRepository
        {
            get
            {
                studentStudyGroupRepository ??= new Repository<StudentStudyGroup>(dbContext);
                return studentStudyGroupRepository;
            }
        }

        public IRepository<Assignment> AssignmentsRepository
        {
            get
            {
                assignmentRepository ??= new Repository<Assignment>(dbContext);
                return assignmentRepository;
            }
        }

        public IRepository<StudentAssignment> StudentAssignmentsRepository
        {
            get
            {
                studentAssignmentRepository ??= new Repository<StudentAssignment>(dbContext);
                return studentAssignmentRepository;
            }
        }

        public IRepository<StudyGroupAssignmentFile> StudyGroupAssignmentFilesRepository
        {
            get
            {
                studentAssignmentFilesRepository ??= new Repository<StudyGroupAssignmentFile>(dbContext);
                return studentAssignmentFilesRepository;
            }
        }

        public IRepository<StudyGroupFile> StudyGroupFilesRepository
        {
            get
            {
                studyGroupFilesRepository ??= new Repository<StudyGroupFile>(dbContext);
                return studyGroupFilesRepository;
            }
        }

        public IRepository<WorkItem> WorkItemsRepository
        {
            get
            {
                workItemsRepository ??= new Repository<WorkItem>(dbContext);
                return workItemsRepository;
            }
        }

        public IRepository<CourseUnit> CourseUnitsRepository
        {
            get
            {
                courseUnitsRepository ??= new Repository<CourseUnit>(dbContext);
                return courseUnitsRepository;
            }
        }

        public IRepository<UnlockedCourseUnit> UnlockedCourseUnitsRepository
        {
            get
            {
                unlockedCourseUnitsRepository ??= new Repository<UnlockedCourseUnit>(dbContext);
                return unlockedCourseUnitsRepository;
            }
        }

        public IRepository<Question> QuestionsRepository
        {
            get
            {
                questionsRepository ??= new Repository<Question>(dbContext);
                return questionsRepository;
            }
        }

        public IRepository<Tag> TagsRepository
        {
            get
            {
                tagsRepository ??= new Repository<Tag>(dbContext);
                return tagsRepository;
            }
        }

        public IRepository<QuestionTag> QuestionTagsRepository
        {
            get
            {
                questionTagsRepository ??= new Repository<QuestionTag>(dbContext);
                return questionTagsRepository;
            }
        }

        public IRepository<Answer> AnswersRepository
        {
            get
            {
                answersRepository ??= new Repository<Answer>(dbContext);
                return answersRepository;
            }
        }

        public IRepository<AnswerVote> AnswerVotesRepository
        {
            get
            {
                answerVotesRepository ??= new Repository<AnswerVote>(dbContext);
                return answerVotesRepository;
            }
        }

        public IRepository<QuestionVote> QuestionVotesRepository
        {
            get
            {
                questionVotesRepository ??= new Repository<QuestionVote>(dbContext);
                return questionVotesRepository;
            }
        }

        public IRepository<Badge> BadgesRepository
        {
            get
            {
                badgesRepository ??= new Repository<Badge>(dbContext);
                return badgesRepository;
            }
        }

        public IRepository<StudentBadge> StudentBadgesRepository
        {
            get
            {
                studentBadgesRepository ??= new Repository<StudentBadge>(dbContext);
                return studentBadgesRepository;
            }
        }

        public IRepository<PostponedAssignment> PostponedAssignmentsRepository
        {
            get
            {
                postponedAssignmentsRepository ??= new Repository<PostponedAssignment>(dbContext);
                return postponedAssignmentsRepository;
            }
        }


        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
