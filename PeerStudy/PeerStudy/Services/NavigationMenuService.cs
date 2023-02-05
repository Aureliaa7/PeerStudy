using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PeerStudy.Services
{
    public class NavigationMenuService : INavigationMenuService
    {
        public List<MenuItem> AdditionalMenuItems { get; set; } = new List<MenuItem>();

        public event EventHandler<EventArgs> OnChanged;

        public void NotifyChanged()
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
