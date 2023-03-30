using Blazored.Toast.Configuration;
using Blazored.Toast.Services;
using PeerStudy.Services.Interfaces;
using System;

namespace PeerStudy.Services
{
    public class PeerStudyToastService : IPeerStudyToastService
    {
        private readonly IToastService toastService;

        public PeerStudyToastService(IToastService toastService)
        {
            this.toastService = toastService;
        }

        public void ClearAll(ToastLevel toastLevel)
        {
            if (toastLevel == ToastLevel.Info)
            {
                toastService.ClearInfoToasts();
            }
            else if (toastLevel == ToastLevel.Error)
            {
                toastService.ClearErrorToasts();
            }
            else if (toastLevel == ToastLevel.Success)
            {
                toastService.ClearSuccessToasts();
            }
        }

        public void ShowToast(ToastLevel toastLevel, string message, bool autoClose = true, bool clearAll = true)
        {
            if (clearAll)
            {
                toastService.ClearAll();
            }

            Action<ToastSettings>? action = null;

            if (!autoClose)
            {
                action = (settings) => settings.DisableTimeout = true;
            }

            if (toastLevel == ToastLevel.Error)
            {
                toastService.ShowError(message, action);
            }
            else if (toastLevel == ToastLevel.Info)
            {
                toastService.ShowInfo(message, action);
            }
            else if (toastLevel == ToastLevel.Success)
            {
                toastService.ShowSuccess(message, action);
            }
        }
    }
}
