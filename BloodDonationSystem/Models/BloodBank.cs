namespace BloodDonationSystem.Models
{
    public class BloodBank
    {
        public int Id { get; set; }

        public int BloodTypeId { get; set; }    
        public BloodType BloodType { get; set; }

        public int quantity { get; set; }

         public ICollection<BloodBankHistory> BloodTakenFromBloodBanks { get; set; }

    }
}
