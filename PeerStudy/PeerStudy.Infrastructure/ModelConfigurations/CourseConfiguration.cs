using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeerStudy.Core.DomainEntities;

namespace PeerStudy.Infrastructure.ModelConfigurations
{
    internal class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(60);

            builder.Property(x => x.NoStudents).HasDefaultValue(default(int));

            builder.HasOne(x => x.Teacher).WithMany(y => y.TeacherCourses);
        }
    }
}
