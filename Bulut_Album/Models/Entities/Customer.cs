using System.ComponentModel.DataAnnotations;

namespace Bulut_Album.Models.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string EmailAdress { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}
