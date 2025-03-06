using System.ComponentModel.DataAnnotations;

namespace Account.Api.Models
{
    public class AccountRequest
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
