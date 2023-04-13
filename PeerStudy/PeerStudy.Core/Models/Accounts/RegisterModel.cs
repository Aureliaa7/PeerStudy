using PeerStudy.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.Accounts
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = Constants.ErrorMessageForShortPassword)]
        public string Password { get; set; }

        public byte[] ProfilePhotoContent { get; set; }

        public Role Role { get; set; }
    }
}
