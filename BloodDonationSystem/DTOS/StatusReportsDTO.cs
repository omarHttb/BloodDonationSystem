using BloodDonationSystem.Models;

namespace BloodDonationSystem.DTOS
{
    public class StatusReportsDTO
    {
        public int TotalDonors { get; set; }
        public int TotalBloodRequests { get; set; }
        public int TotalDonations { get; set; }

        public List<BloodBank> BloodBanks { get; set; }
    }
}
