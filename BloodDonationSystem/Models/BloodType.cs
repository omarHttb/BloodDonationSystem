namespace BloodDonationSystem.Models
{
    public class BloodType
    {
        public int Id { get; set; }

        public string BloodTypeName { get; set; }

        public ICollection<Donor> Donors { get; set; }

        public ICollection<BloodRequestBloodType> BloodRequestBloodTypes { get; set; }


    }
}
