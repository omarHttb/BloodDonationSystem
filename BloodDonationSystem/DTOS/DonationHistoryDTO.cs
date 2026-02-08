namespace BloodDonationSystem.DTOS
{
    public class DonationHistoryDTO
    {
        public int quantity { get; set; }

        public string Status { get; set; }

        public int bloodRequestId { get;set; }

        public string BloodRequestBloodType { get; set; }

        public DateTime DonationSubmitDate { get; set; }

        public DateTime? DonationDate { get; set; }

        public DateOnly? DateDonorChoseToDonate { get; set; }
    }
}
