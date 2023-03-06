namespace PeerStudy.Core.Models.Resources
{
    public class UploadDriveFileModel : UploadFileModel
    {
        public string OwnerEmail { get; set; }

        public string Type { get; set; }

        public string ParentFolderId { get; set; }
    }
}
