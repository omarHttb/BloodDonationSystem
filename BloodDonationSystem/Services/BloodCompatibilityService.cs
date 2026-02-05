using BloodDonationSystem.Services.Interfaces;

namespace BloodDonationSystem.Services
{
    public class BloodCompatibilityService : IBloodCompatibilityService
    {
        // Key = Donor, Value = List of compatible Patients
        private readonly Dictionary<string, List<string>> _donationRules = new()
    {
        { "O-",  new List<string> { "O-", "O+", "A-", "A+", "B-", "B+", "AB-", "AB+" } }, // Universal Donor
        { "O+",  new List<string> { "O+", "A+", "B+", "AB+" } },
        { "A-",  new List<string> { "A-", "A+", "AB-", "AB+" } },
        { "A+",  new List<string> { "A+", "AB+" } },
        { "B-",  new List<string> { "B-", "B+", "AB-", "AB+" } },
        { "B+",  new List<string> { "B+", "AB+" } },
        { "AB-", new List<string> { "AB-", "AB+" } },
        { "AB+", new List<string> { "AB+" } } // Universal Recipient
    };
        public bool CanDonate(string donorBloodType, string patientBloodType)
        {
            var cleanDonor = donorBloodType?.Trim().ToUpper();
            var cleanPatient = patientBloodType?.Trim().ToUpper();

            if (string.IsNullOrEmpty(cleanDonor) || string.IsNullOrEmpty(cleanPatient))
                return false;

            if (_donationRules.TryGetValue(cleanDonor, out var compatiblePatients))
            {
                return compatiblePatients.Contains(cleanPatient);
            }

            return false; // Unknown blood type
        }
    }
}
