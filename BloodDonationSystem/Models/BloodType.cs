namespace BloodDonationSystem.Models
{
    public class BloodType
    {
        public int Id { get; set; }

        public string BloodTypeName { get; set; }

        public ICollection<Donor> Donors { get; set; }

        public ICollection<BloodRequest> BloodRequests { get; set; }

        public ICollection<BloodBank> BloodBanks { get; set; }  


    }
}
