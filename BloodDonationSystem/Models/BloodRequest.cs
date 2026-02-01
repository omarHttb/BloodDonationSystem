namespace BloodDonationSystem.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        public DateTime BloodRequestDate { get; set; }

        public bool isApproved { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Donation> Donations { get; set; }

        public ICollection<BloodRequestBloodType> bloodRequestBloodTypes { get; set; }


    }
}
