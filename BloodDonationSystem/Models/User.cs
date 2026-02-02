using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodDonationSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string password { get; set; }

        [NotMapped]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public Donor Donors { get; set; }

    }
}
