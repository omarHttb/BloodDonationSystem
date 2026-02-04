namespace BloodDonationSystem.DTOS
{
    public class ApprovedBloodRequestDTO
    {
        public int Id { get; set; }
        public string BloodTypeName { get; set; }

        public DateTime BloodRequestDate { get; set; }

        public bool IsBloodRequestActive { get; set; }

        public string Status { get; set; }

        public int QuantityRequested { get; set; }

 



    }
}
