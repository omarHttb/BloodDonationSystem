namespace BloodDonationSystem.DTOS
{
    public class CreateBloodRequestDTO
    {
        public List<BloodTypeSelectionDTO> BloodTypes { get; set; } = new List<BloodTypeSelectionDTO>();

        public IEnumerable<BloodRequestsDTO> BloodRequests { get; set; }

        public int SelectedBloodTypeId { get; set; }


    }
}
