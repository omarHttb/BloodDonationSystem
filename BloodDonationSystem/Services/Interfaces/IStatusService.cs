using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IStatusService 
    {
        Task<Status> GetStatusByIdAsync(int id);
        Task<List<Status>> GetAllStatusAsync();
        Task<Status> CreateStatusAsync(Status status);
        Task<Status> UpdateStatusAsync(int id, Status status);
        Task<bool> DeleteStatusAsync(int id);
    }
}