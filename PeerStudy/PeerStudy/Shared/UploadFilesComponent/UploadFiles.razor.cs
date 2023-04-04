using MatBlazor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PeerStudy.Shared.UploadFilesComponent
{
    public partial class UploadFiles
    {
        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<List<UploadFileModel>> OnUpload { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        private const string uploadFileControlStyles = "border-radius: 30px; background-color: #F5F5F5; height: 30px;";
        private IMatFileUploadEntry[] uploadedFiles;

        private async Task Cancel()
        {
            await OnCancel.InvokeAsync();
        }

        private async Task Upload()
        {
            var files = await GetUploadFilesModelsAsync();
            uploadedFiles = null;
            await OnUpload.InvokeAsync(files);
        }

        private bool IsUploadFileButtonEnabled()
        {
            return uploadedFiles != null;
        }

        private async Task<List<UploadFileModel>> GetUploadFilesModelsAsync()
        {
            var uploadFileModels = new List<UploadFileModel>();

            foreach (var file in uploadedFiles)
            {
                using (var stream = new MemoryStream())
                {
                    await file.WriteToStreamAsync(stream);
                    uploadFileModels.Add(new UploadFileModel
                    {
                        FileContent = stream.ToArray(),
                        Name = file.Name
                    });
                }
            }

            return uploadFileModels;
        }

        private void GetUploadedFiles(IMatFileUploadEntry[] files)
        {
            uploadedFiles = files;
        }
    }
}
