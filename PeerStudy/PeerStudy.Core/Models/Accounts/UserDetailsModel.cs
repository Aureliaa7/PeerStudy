using System;

namespace PeerStudy.Core.Models.Accounts
{
    public class UserDetailsModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ProfilePhotoName { get; set; }

    }
}
