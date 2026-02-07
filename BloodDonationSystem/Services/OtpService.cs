using BloodDonationSystem.Services.Interfaces;
using System;
using System.Collections.Concurrent;

namespace BloodDonationSystem.Services
{
    public class OtpService : IOtpService
    {
        private readonly ConcurrentDictionary<string, DateTime> _otpCache = new();
        private readonly Random _random = new();
        public async Task<string> GenerateOtpAsync()
        {
            const string numbers = "0123456789";
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ"; // Omitted 'I' and 'O' for clarity

            var otpList = Enumerable.Range(0, 3)
                .Select(_ => numbers[_random.Next(numbers.Length)])
                .ToList();

            // 2. Generate 2 random characters
            otpList.AddRange(Enumerable.Range(0, 2)
                .Select(_ => chars[_random.Next(chars.Length)]));

            var finalOtp = new string(otpList.OrderBy(x => _random.Next()).ToArray());

            _otpCache.TryAdd(finalOtp, DateTime.UtcNow.AddMinutes(5));

            return await Task.FromResult(finalOtp);
        }

        public async Task<bool> ValidateOtpAsync(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp)) return false;

            // Check if OTP exists and hasn't expired
            if (_otpCache.TryGetValue(otp, out var expiry))
            {
                if (DateTime.UtcNow <= expiry)
                {
                    // OTP is valid! Remove it so it can't be used again
                    _otpCache.TryRemove(otp, out _);
                    return await Task.FromResult(true);
                }
                else
                {
                    // Expired
                    _otpCache.TryRemove(otp, out _);
                }
            }

            return await Task.FromResult(false);
        }
    }
}

