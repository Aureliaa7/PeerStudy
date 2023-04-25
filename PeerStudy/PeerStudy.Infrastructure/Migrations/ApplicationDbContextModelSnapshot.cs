﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PeerStudy.Infrastructure.AppDbContext;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Answer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.AnswerVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AnswerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("VoteType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.HasIndex("AuthorId");

                    b.ToTable("AnswerVotes", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CourseUnitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<Guid>("StudyGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CourseUnitId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("Assignments", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AssignmentsDriveFolderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DriveRootFolderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("HasStudyGroups")
                        .HasColumnType("bit");

                    b.Property<int>("NoStudents")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("ResourcesDriveFolderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("StudyGroupsDriveFolderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TeacherId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("Id");

                    b.HasIndex("TeacherId");

                    b.ToTable("Courses", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseEnrollmentRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("CourseEnrollmentRequests", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseResource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseUnitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DriveFileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("CourseUnitId");

                    b.HasIndex("OwnerId");

                    b.ToTable("CourseResources", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<int>("NoPointsToUnlock")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseUnits", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Questions", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.QuestionTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("TagId");

                    b.ToTable("QuestionTags", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.QuestionVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("VoteType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionVotes", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentAsset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Asset")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfAssets")
                        .HasColumnType("int");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("StudentAssets", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Points")
                        .HasColumnType("int");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudyGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("StudentId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("StudentAssignments", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentCourse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("StudentCourses", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentStudyGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudyGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("StudentStudyGroup", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DriveFolderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("StudyGroups", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroupAssignmentFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DriveFileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.ToTable("StudyGroupAssignmentFiles", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroupFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DriveFileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudyGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("StudyGroupFiles", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.UnlockedCourseUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseUnitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CourseUnitId");

                    b.HasIndex("StudentId");

                    b.ToTable("UnlockedCourseUnits", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ProfilePhotoName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.WorkItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssignedToId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("StudyGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("WorkItems", (string)null);
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Student", b =>
                {
                    b.HasBaseType("PeerStudy.Core.DomainEntities.User");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Teacher", b =>
                {
                    b.HasBaseType("PeerStudy.Core.DomainEntities.User");

                    b.HasDiscriminator().HasValue("Teacher");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Answer", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Author")
                        .WithMany("Answers")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.AnswerVote", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Answer", "Answer")
                        .WithMany("Votes")
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Author")
                        .WithMany("AnswerVotes")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Assignment", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.CourseUnit", "CourseUnit")
                        .WithMany("Assignments")
                        .HasForeignKey("CourseUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.StudyGroup", "StudyGroup")
                        .WithMany("Assignments")
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CourseUnit");

                    b.Navigation("StudyGroup");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Course", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Teacher", "Teacher")
                        .WithMany("TeacherCourses")
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseEnrollmentRequest", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseResource", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.CourseUnit", "CourseUnit")
                        .WithMany("Resources")
                        .HasForeignKey("CourseUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Teacher", "Owner")
                        .WithMany("CourseResources")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("CourseUnit");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseUnit", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Course", "Course")
                        .WithMany("CourseUnits")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Question", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Author")
                        .WithMany("Questions")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.QuestionTag", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Question", "Question")
                        .WithMany("QuestionTags")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Tag", "Tag")
                        .WithMany("QuestionTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.QuestionVote", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Author")
                        .WithMany("QuestionVotes")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentAsset", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany("Assets")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentAssignment", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Assignment", "Assignment")
                        .WithMany("StudentAssignments")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany("Assignments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.StudyGroup", "StudyGroup")
                        .WithMany()
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");

                    b.Navigation("Student");

                    b.Navigation("StudyGroup");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentCourse", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Course", "Course")
                        .WithMany("CourseEnrollments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany("CourseEnrollments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudentStudyGroup", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany("StudentStudyGroups")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.StudyGroup", "StudyGroup")
                        .WithMany("StudentStudyGroups")
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");

                    b.Navigation("StudyGroup");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroup", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroupAssignmentFile", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroupFile", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Owner")
                        .WithMany("StudyGroupFiles")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.StudyGroup", "StudyGroup")
                        .WithMany("StudyGroupFiles")
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("StudyGroup");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.UnlockedCourseUnit", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.CourseUnit", "CourseUnit")
                        .WithMany()
                        .HasForeignKey("CourseUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "Student")
                        .WithMany("UnlockedCourseUnits")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CourseUnit");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.WorkItem", b =>
                {
                    b.HasOne("PeerStudy.Core.DomainEntities.Student", "AssignedTo")
                        .WithMany("WorkItems")
                        .HasForeignKey("AssignedToId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("PeerStudy.Core.DomainEntities.StudyGroup", "StudyGroup")
                        .WithMany("WorkItems")
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedTo");

                    b.Navigation("StudyGroup");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Answer", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Assignment", b =>
                {
                    b.Navigation("StudentAssignments");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Course", b =>
                {
                    b.Navigation("CourseEnrollments");

                    b.Navigation("CourseUnits");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.CourseUnit", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Question", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("QuestionTags");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.StudyGroup", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("StudentStudyGroups");

                    b.Navigation("StudyGroupFiles");

                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Tag", b =>
                {
                    b.Navigation("QuestionTags");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Student", b =>
                {
                    b.Navigation("AnswerVotes");

                    b.Navigation("Answers");

                    b.Navigation("Assets");

                    b.Navigation("Assignments");

                    b.Navigation("CourseEnrollments");

                    b.Navigation("QuestionVotes");

                    b.Navigation("Questions");

                    b.Navigation("StudentStudyGroups");

                    b.Navigation("StudyGroupFiles");

                    b.Navigation("UnlockedCourseUnits");

                    b.Navigation("WorkItems");
                });

            modelBuilder.Entity("PeerStudy.Core.DomainEntities.Teacher", b =>
                {
                    b.Navigation("CourseResources");

                    b.Navigation("TeacherCourses");
                });
#pragma warning restore 612, 618
        }
    }
}
