using PeerStudy.Models;
using System;
using System.Collections.Generic;

namespace PeerStudy.Services.Interfaces
{
    public interface INavigationMenuService
    {
        List<MenuItem> GetMenuItems();

        void AddMenuItems(List<MenuItem> items);

        void RemoveAll();

        event EventHandler<EventArgs> OnChanged;
        
        void NotifyChanged();
    }
}
