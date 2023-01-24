using MatBlazor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Components.Accounts
{
    public partial class Register
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IAccountService AccountService { get; set; }

        private RegisterModel registerModel = new();
        private string errorMessage;
        private bool showErrorMessage;

        private IMatFileUploadEntry profilePicture;

        private async Task CreateAccount()
        {
            var profilePhotoContent = await GetProfilePhotoContentAsync();
            if (profilePhotoContent != null)
            {
                registerModel.ProfilePhotoContent = profilePhotoContent;
            }

            registerModel.Role = Role.Student;

            try
            {
                var result = await AccountService.RegisterAsync(registerModel);

                if (result != null)
                {
                    NavigationManager.NavigateTo("/login");
                }
            }
            catch (DuplicateEntityException)
            {
                HandleException("A user with the same email already exists!");
            }
            catch (Exception)
            {
                HandleException("An error occurred while creating the account...");
            }
        }

        private void HandleException(string message)
        {
            errorMessage = message;
            showErrorMessage = true;
            registerModel = new();
        }

        private async Task<byte[]> GetProfilePhotoContentAsync()
        {
            if (profilePicture != null)
            {
                using (var stream = new MemoryStream())
                {
                    await profilePicture.WriteToStreamAsync(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }

        void FilesReady(IMatFileUploadEntry[] files)
        {
            profilePicture = files.First();
        }
    }
}
