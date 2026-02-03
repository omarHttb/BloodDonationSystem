namespace BloodDonationSystem.DTOS
{
    public class BloodRequestsDTO
    {
        public int Id { get; set; }
        public string BloodTypeName { get; set; }

        public DateTime BloodRequestDate { get; set; }

        public bool IsBloodRequestActive { get; set; }

        public bool IsBloodRequestApproved { get; set; }

        public int QuantityRequested { get; set; }

    }
}
