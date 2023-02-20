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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEntities(modelBuilder);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            new UserConfiguration().Configure(modelBuilder.Entity<User>());
            new CourseConfiguration().Configure(modelBuilder.Entity<Course>());
            new StudentStudyGroupConfiguration().Configure(modelBuilder.Entity<StudentStudyGroup>());
        }
    }
}
