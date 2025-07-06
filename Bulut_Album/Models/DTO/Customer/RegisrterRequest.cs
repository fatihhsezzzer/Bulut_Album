using System.ComponentModel.DataAnnotations;

namespace Bulut_Album.Models.DTO.Customer
{
    public class RegisterRequest
    {
       

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
