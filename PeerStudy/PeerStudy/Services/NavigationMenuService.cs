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
