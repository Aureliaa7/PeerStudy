using PeerStudy.Core.Enums;
using PeerStudy.Models;
using System;
using System.Collections.Generic;

namespace PeerStudy.Services.Interfaces
{
    public interface INavigationMenuService
    {
        List<MenuItem> GetMenuItems();

        void AddMenuItems(List<MenuItem> items);

        void AddCourseNavigationMenuItems(Guid userId, Guid courseId, string courseTitle, Role? userRole);

        void AddStudyGroupNavigationMenuItems(Guid studyGroupId, string studyGroupTitle);

        void Reset();

        public string CurrentUsername { get; set; }

        event EventHandler<EventArgs> OnChanged;
        
        void NotifyChanged();
    }
}
