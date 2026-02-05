namespace BloodDonationSystem.Services.Interfaces
{
    public interface IBloodCompatibilityService
    {
        bool CanDonate(string donorBloodType, string patientBloodType);

    }
}
