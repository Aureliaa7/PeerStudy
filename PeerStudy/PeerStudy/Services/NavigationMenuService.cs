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

        public void AddNavigationMenuItemsForStudent(NavigationDataModel data)
        {
            additionalMenuItems.AddRange(new List<MenuItem> {
                new MenuItem
                {
                    Href = $"/{data.UserId}/my-courses/{data.CourseId}/resources",
                    Name = "Resources"
                },
                  new MenuItem
                {
                    Href = $"{data.CourseTitle}/{data.CourseId}/{data.UserId}/my-assignments",
                    Name = "Assignments"
                } }
            );

            NotifyChanged();
        }

        public void AddNavigationMenuItemsForTeacher(NavigationDataModel data)
        {
            additionalMenuItems.AddRange(new List<MenuItem> {
                new MenuItem
                {
                    Href = $"/{data.UserId}/courses/{data.CourseId}/resources",
                    Name = "Resources"
                },
                  new MenuItem
                {
                    Href = $"/{data.CourseTitle}/{data.CourseId}/assignments",
                    Name = "Assignments"
                },
                new MenuItem
                {
                    Href = $"/courses/{data.CourseTitle}/{data.CourseId}/students",
                    Name = "Students"
                },
                new MenuItem
                {
                    Href = $"/{data.UserId}/courses/{data.CourseTitle}/{data.CourseId}/pending-requests",
                    Name = "Pending requests"
                },
                new MenuItem
                {
                    Href = $"/{data.UserId}/courses/{data.CourseTitle}/{data.CourseId}/rejected-requests",
                    Name = "Rejected requests"
                }
            });

            NotifyChanged();
        }

        public List<MenuItem> GetMenuItems()
        {
            return additionalMenuItems;
        }

        public void NotifyChanged()
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveAll()
        {
            additionalMenuItems.Clear();
        }
    }
}
