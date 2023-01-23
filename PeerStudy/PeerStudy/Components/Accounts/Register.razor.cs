using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models;
using System;
using System.IO;
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
        private IBrowserFile imageFile;
        private string errorMessage;
        private bool showErrorMessage;

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

        private void HandleSelected(InputFileChangeEventArgs e)
        {
            imageFile = e.File;
        }

        private async Task<byte[]> GetProfilePhotoContentAsync()
        {
            if (imageFile != null)
            {
                var resizedFile = await imageFile.RequestImageFileAsync(
                    "image/png", ClientConstants.ImageWidth, ClientConstants.ImageHeight);
                using (var source = resizedFile.OpenReadStream(resizedFile.Size))
                {
                    using (var stream = new MemoryStream())
                    {
                        await source.CopyToAsync(stream);
                        return stream.ToArray();
                    }
                }
            }
            return null;
        }
    }
}
