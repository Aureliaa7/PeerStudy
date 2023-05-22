using PeerStudy.Core.Enums;
using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PeerStudy.Services
{
    public class NavigationMenuService : INavigationMenuService
    {
        private List<MenuItem> additionalMenuItems = new List<MenuItem>();

        public string CurrentUsername { get; set; }

        public event EventHandler<EventArgs> OnChanged;

        public void AddMenuItems(List<MenuItem> items)
        {
            foreach (MenuItem item in items)
            {
                bool itemExists = additionalMenuItems.Any(x => x.Name == item.Name);
                if (!itemExists)
                {
                    additionalMenuItems.Add(item);
                }
            }
        }

        public void AddCourseNavigationMenuItems(Guid userId, Guid courseId, string courseTitle, Role? userRole)
        {
            if (userRole == Role.Teacher)
            {
                AddMenuItems(GetCourseMenuItemsForTeacher(userId, courseId, courseTitle));
            }
            else if (userRole == Role.Student)
            {
                AddMenuItems(GetCourseMenuItemsForStudent(userId, courseId, courseTitle));
            }
            NotifyChanged();
        }

        private List<MenuItem> GetCourseMenuItemsForStudent(Guid userId, Guid courseId, string courseTitle)
        {
            return new List<MenuItem> {
                new MenuItem
                {
                    Href = $"/{userId}/my-courses/{courseId}/home",
                    Name = "Home"
                },
                new MenuItem
                {
                    Href = $"{courseTitle}/{courseId}/{userId}/my-assignments",
                    Name = "Assignments"
                }
            };
        }

        private List<MenuItem> GetCourseMenuItemsForTeacher(Guid userId, Guid courseId, string courseTitle)
        {
            return new List<MenuItem> {
                new MenuItem
                {
                    Href = $"/{userId}/courses/{courseId}/home",
                    Name = "Home"
                },
                new MenuItem
                {
                    Href = $"/courses/{courseTitle}/{courseId}/students",
                    Name = "Students"
                },
                new MenuItem
                {
                    Href = $"/{userId}/courses/{courseTitle}/{courseId}/pending-requests",
                    Name = "Pending requests"
                },
                new MenuItem
                {
                    Href = $"/{userId}/courses/{courseTitle}/{courseId}/rejected-requests",
                    Name = "Rejected requests"
                },
                new MenuItem
                {
                    Href = $"/{courseTitle}/{courseId}/students-progress",
                    Name = "Leaderboards"
                }
            };
        }

        public List<MenuItem> GetMenuItems()
        {
            return additionalMenuItems;
        }

        public void NotifyChanged()
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            additionalMenuItems.Clear();
            NotifyChanged();
        }

        public void AddStudyGroupNavigationMenuItems(Guid studyGroupId, string studyGroupTitle, Role userRole)
        {
            AddMenuItems(new List<MenuItem>
            {
                new MenuItem
                {
                    Href = $"/{studyGroupId}/home",
                    Name = "Home"
                },
                new MenuItem
                {
                    Href = $"{studyGroupTitle}/{studyGroupId}/work-items",
                    Name = "Tasks"
                }
            });

            if (userRole == Role.Teacher)
            {
                AddMenuItems(new List<MenuItem>
                {
                    new MenuItem
                    {
                        Name = "Assignments",
                        Href = $"/{studyGroupId}/{studyGroupTitle}/assignments"
                    }
                });
            }

            NotifyChanged();
        }
    }
}
