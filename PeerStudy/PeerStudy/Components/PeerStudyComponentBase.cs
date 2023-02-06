using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Components
{
    public abstract class PeerStudyComponentBase<T> : ComponentBase where T: new()
    {
        [Inject]
        IAuthService AuthService { get; set; }

        protected List<T> data;

        protected Guid currentUserId;
        protected bool isTeacher;
        protected bool isStudent;
        protected bool isLoading;

        protected async Task InitializeDataAsync()
        {
            isLoading = true;
            await InitializeAsync();
            data = await GetDataAsync();
            isLoading = false;
        }

        private async Task InitializeAsync()
        {
            currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
            string userRole = await AuthService.GetCurrentUserRole();
            isTeacher = userRole == Role.Teacher.ToString();
            isStudent = userRole == Role.Student.ToString();
        }

        protected abstract Task<List<T>> GetDataAsync();
    }
}
