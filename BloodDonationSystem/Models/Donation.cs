namespace BloodDonationSystem.Models
{
    public class Donation
    {
        public int Id { get; set; }

        public int DonorId { get; set; }

        public Donor Donor { get; set; }

        public int Quantity { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        public DateTime DonationDate { get; set; }

        public int BloodRequestId { get; set; }
    
        public BloodRequest BloodRequest { get; set; }
    }
}
