using BloodDonationSystem.Models;

namespace BloodDonationSystem.DTOS
{
    public class DonorsAndBloodTypesDTO
    {
        public List<DonorDTO> Donors { get; set; }

        public List<BloodType> bloodTypes { get; set; }

    }
}
