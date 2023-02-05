using PeerStudy.Models;
using System;
using System.Collections.Generic;

namespace PeerStudy.Services.Interfaces
{
    public interface INavigationMenuService
    {
        List<MenuItem> AdditionalMenuItems { get; set; }

        event EventHandler<EventArgs> OnChanged;
        
        void NotifyChanged();
    }
}
