namespace BloodDonationSystem.Models
{
    public class UserRole
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime? RoleAssignDate { get; set; }

    }
}
