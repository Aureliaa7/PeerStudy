namespace PeerStudy.Core.Models.Resources
{
    public class UploadFileModel
    {
        public string Name { get; set; }

        public byte[] FileContent { get; set; }

        public string OwnerEmail { get; set; }

        public string Type { get; set; }

        public string ParentFolderId { get; set; }
    }
}
