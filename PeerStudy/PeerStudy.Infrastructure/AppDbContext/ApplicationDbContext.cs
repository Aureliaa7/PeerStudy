using Microsoft.EntityFrameworkCore;
using PeerStudy.Core.DomainEntities;
using PeerStudy.Infrastructure.ModelConfigurations;

namespace PeerStudy.Infrastructure.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseEnrollmentRequest> CourseEnrollmentRequests { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<CourseResource> CourseResources { get; set; }

        public DbSet<StudyGroup> StudyGroups { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<StudentAssignment> StudentAssignments { get; set; }

        public DbSet<StudyGroupAssignmentFile> StudyGroupAssignmentFiles { get; set; }

        public DbSet<WorkItem> WorkItems { get; set; }

        public DbSet<StudyGroupFile> StudyGroupFiles { get; set; }

        public DbSet<CourseUnit> CourseUnits { get; set; }

        public DbSet<UnlockedCourseUnit> UnlockedCourseUnits { get; set; }

        public DbSet<StudentAsset> StudentAssets { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<QuestionTag> QuestionTags { get; set; }

        public DbSet<AnswerVote> AnswerVotes { get; set; }

        public DbSet<QuestionVote> QuestionVotes { get; set; }

        public DbSet<Badge> Badges { get; set; }

        public DbSet<StudentBadge> StudentBadges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEntities(modelBuilder);
        }

        private static void ConfigureEntities(ModelBuilder modelBuilder)
        {
            new UserConfiguration().Configure(modelBuilder.Entity<User>());
            new CourseConfiguration().Configure(modelBuilder.Entity<Course>());
            new StudentStudyGroupConfiguration().Configure(modelBuilder.Entity<StudentStudyGroup>());
            new AssignmentConfiguration().Configure(modelBuilder.Entity<Assignment>());
            new StudentAssignmentConfiguration().Configure(modelBuilder.Entity<StudentAssignment>());
            new WorkItemConfiguration().Configure(modelBuilder.Entity<WorkItem>());
            new CourseResourceConfiguration().Configure(modelBuilder.Entity<CourseResource>());
            new StudyGroupFileConfiguration().Configure(modelBuilder.Entity<StudyGroupFile>());
            new CourseUnitConfiguration().Configure(modelBuilder.Entity<CourseUnit>());
            new AnswerConfiguration().Configure(modelBuilder.Entity<Answer>());
            new AnswerVoteConfiguration().Configure(modelBuilder.Entity<AnswerVote>());
            new QuestionVoteConfiguration().Configure(modelBuilder.Entity<QuestionVote>());
            new BadgeConfiguration().Configure(modelBuilder.Entity<Badge>());
            new StudentBadgeConfiguration().Configure(modelBuilder.Entity<StudentBadge>());
        }
    }
}
