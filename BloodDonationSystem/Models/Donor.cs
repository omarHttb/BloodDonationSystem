namespace BloodDonationSystem.Models
{
    public class Donor
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public User User { get; set; }

        public int BloodTypeId { get; set; }

        public bool IsAvailable { get; set; }

        public BloodType BloodType { get; set; }

        public ICollection<Donation> Donations { get; set; }

    }
}
