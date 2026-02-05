namespace BloodDonationSystem.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        public DateTime BloodRequestDate { get; set; }

        public bool isApproved { get; set; }

        public bool IsActive { get; set; }

        public int BloodTypeId { get; set; }

        public BloodType BloodType { get; set; }

        public int Quantity { get; set; }

        public ICollection<Donation> Donations { get; set; }



    }
}
