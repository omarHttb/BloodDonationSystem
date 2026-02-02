namespace BloodDonationSystem.Models
{
    public class BloodRequestBloodType
    {
        public int Id { get; set; }

        public int BloodTypeId { get; set; }

        public BloodType BloodType { get; set; }

        public int BloodRequestId { get; set; }

        public BloodRequest BloodRequest { get; set; }

        public int Quantity { get; set; }
    }
}
