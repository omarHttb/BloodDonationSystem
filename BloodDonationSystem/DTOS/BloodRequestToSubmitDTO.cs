namespace BloodDonationSystem.DTOS
{
    public class BloodRequestToSubmitDTO
    {

        public List<ApprovedBloodRequestDTO> ApprovedBloodRequests { get; set; }
        public bool isUserADonor { get; set; }
        public bool IsDonorAvailableToDonate { get; set; }   
        public bool DoesUserHaveBloodType { get; set; }

        public DateOnly? WhenUserWantToDonate { get; set; }
        public bool? DidUserCompleteHisDonationTimeLimit { get; set; }



    }
}
