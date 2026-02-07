namespace BloodDonationSystem.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync();

        Task<bool> ValidateOtpAsync(string otp);

    }
}
