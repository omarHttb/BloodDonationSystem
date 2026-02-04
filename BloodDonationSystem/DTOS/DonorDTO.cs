using BloodDonationSystem.Models;

namespace BloodDonationSystem.DTOS
{
    public class DonorDTO
    {
        public int DonorId { get; set; }
        public string DonorName { get; set; }
        public bool isAvailable { get; set; }   
        public string BloodType { get; set; }
    }
}
