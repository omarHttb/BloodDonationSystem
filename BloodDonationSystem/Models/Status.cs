namespace BloodDonationSystem.Models
{
    public class Status
    {
        public int Id { get; set; }

        public string StatusName { get; set; }

        public ICollection<Donation> Donations { get; set; }
    }
}
