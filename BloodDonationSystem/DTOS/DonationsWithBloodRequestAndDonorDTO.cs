namespace BloodDonationSystem.DTOS
{
    public class DonationsWithBloodRequestAndDonorDTO
    {
        public int DonationId { get; set; }

        public int BloodRequestId { get; set; }

        public string RequestedBloodType { get; set; }

        public string DonatorName { get; set; }
        public string DonatorEmail { get; set; }
        public string DonatorPhoneNumber { get; set; }

        public string DonatorBloodType { get; set; }

        public string DonationStatus { get; set; }

        public int QuantityRequested { get; set; }

        public int DonationQuantity { get; set; }

        public DateTime DonationDateSubmitted {  get; set; }

        public DateTime BloodRequestDate {  get; set; }

        public bool IsDonationActive { get; set; }

        public bool IsDonatorAvailable { get; set; }

        public DateTime? DonationDate {  get; set; }

     
    }
}
