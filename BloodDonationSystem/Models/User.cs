namespace BloodDonationSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string password { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public Donor Donors { get; set; }

    }
}
