using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Students.Components.HomePageComponent
{
    public partial class StudentHomePage : PeerStudyComponentBase
    {
        [Inject]
        private IAchievementService AchievementService { get; set; }


        private StudentProfileModel studentProgress;

        private const string noCompletedAssignmentsMessage = "There are no completed assignments yet...";
        private const string noUnlockedCourseUnits = "There are no unlocked course units yet...";
        private const string noProgressMessage = "No progress yet...";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            studentProgress = await AchievementService.GetProgressByStudentIdAsync(currentUserId);
        }

        //TODO: delete
        private StudentProfileModel StudentProfileModel = new StudentProfileModel
        {
            Email = "user1@test.com",
            Name = "My test user",
            NoTotalPoints = 50,
            CoursesProgress = new System.Collections.Generic.List<Core.Models.Courses.StudentCourseProgressModel>
            {
                new Core.Models.Courses.StudentCourseProgressModel
                {
                    CourseTitle = "Course 1",
                    TeacherName = "Teacher 1",
                    CourseUnitsAssignmentsProgress = new System.Collections.Generic.List<Core.Models.Assignments.StudentCourseUnitAssignmentsModel>
                    {
                        new Core.Models.Assignments.StudentCourseUnitAssignmentsModel
                        {
                            CourseUnitTitle = "Course Unit 1",
                            StudentAssignments = new System.Collections.Generic.List<Core.Models.Assignments.StudentAssignmentDetailsModel>
                            {
                                new Core.Models.Assignments.StudentAssignmentDetailsModel
                                {
                                    AssignmentTitle = "Assignment 1",
                                    CompletedAt = DateTime.UtcNow,
                                    NoEarnedPoints = 50
                                }
                            }
                        }
                    },
                    UnlockedCourseUnits = new System.Collections.Generic.List<Core.Models.CourseUnits.UnlockedCourseUnitModel>
            {
                new Core.Models.CourseUnits.UnlockedCourseUnitModel
                {
                    UnlockedAt = System.DateTime.UtcNow,
                    CourseUnitTitle = "course unit 1",
                    NoPaidPoints = 10
                }

                    }
                }
            }
        };

        private bool HasCompletedAssignments()
        {
            return studentProgress.CoursesProgress
                .Any(x => x.CourseUnitsAssignmentsProgress.Any(y => y.StudentAssignments.Any()));
        }
    }
}
