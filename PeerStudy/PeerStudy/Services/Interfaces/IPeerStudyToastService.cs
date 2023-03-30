using Blazored.Toast.Services;

namespace PeerStudy.Services.Interfaces
{
    public interface IPeerStudyToastService
    {
        void ShowToast(ToastLevel toastLevel, string message, bool autoClose = true, bool clearAll = true);

        void ClearAll(ToastLevel toastLevel);
    }
}
