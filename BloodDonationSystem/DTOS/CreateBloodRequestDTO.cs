namespace BloodDonationSystem.DTOS
{
    public class CreateBloodRequestDTO
    {
        // The list of checkboxes
        public List<BloodTypeSelectionDTO> BloodTypes { get; set; }

        public List<BloodRequestsDTO> BloodRequests { get; set; }
    
    }
}
