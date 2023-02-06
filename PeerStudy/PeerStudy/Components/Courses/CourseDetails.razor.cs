using Microsoft.AspNetCore.Components;
using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PeerStudy.Components.Courses
{
    public partial class CourseDetails
    {
        [Inject]
        INavigationMenuService NavigationMenuService { get; set; }  

        [Parameter]
        public Guid TeacherId { get; set; }

        [Parameter]
        public Guid StudentId { get; set; }

        [Parameter]
        public Guid CourseId { get; set; }


        public CourseDetails()
        {
        
        }

        protected override void OnInitialized()
        {
            //TODO: to be implemented
            NavigationMenuService.AdditionalMenuItems.AddRange(new List<MenuItem> {
                  new MenuItem
                    {
                        Href = "#",
                        Name = "Resources"
                    },
                    new MenuItem
                    {
                        Href = "#",
                        Name = "Students"
                    },
                    new MenuItem
                    {
                        Href = "#",
                        Name = "Study groups"
                    },
                       new MenuItem
                    {
                        Href = $"/{TeacherId}/courses{CourseId}/pending-requests",
                        Name = "Pending requests"
                    }
                });
            NavigationMenuService.NotifyChanged();
        }
    }
}
