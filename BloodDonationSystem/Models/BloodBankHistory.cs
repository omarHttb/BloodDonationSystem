namespace BloodDonationSystem.Models
{
    public class BloodBankHistory
    {
        public int Id { get; set; }

        public int BloodBankId { get; set; }

        public BloodBank BloodBank { get; set; }

        public int QuantityTransaction { get; set; }

        public bool IsBloodAdded { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
