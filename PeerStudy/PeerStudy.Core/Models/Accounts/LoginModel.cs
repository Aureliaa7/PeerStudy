using System.ComponentModel.DataAnnotations;

namespace PeerStudy.Core.Models.Accounts
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = Constants.ErrorMessageForShortPassword)]
        public string Password { get; set; }
    }
}
