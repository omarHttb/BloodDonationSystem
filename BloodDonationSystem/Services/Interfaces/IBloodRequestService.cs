using BloodDonationSystem.DTOS;
using BloodDonationSystem.Models;

namespace BloodDonationSystem.Services.Interfaces

{
    public interface IBloodRequestService 
    {
        Task<BloodRequest> GetBloodRequestByIdAsync(int id);
        Task<List<BloodRequest>> GetAllBloodRequestWithBloodTypesAsync();

        Task<BloodRequestToSubmitDTO> GetAllApprovedBloodRequest(int userId);

        Task<List<BloodRequest>> GetAllBloodRequestAsnyc();
        Task<BloodRequest> CreateBloodRequestAsync(BloodRequest bloodRequest);
        Task<BloodRequest> UpdateBloodRequestAsync(int id, BloodRequest bloodRequest);
        Task<bool> DeleteBloodRequestAsync(int id);

        Task<bool> ApproveBloodRequest(int id);
        Task<bool> DisApproveBloodRequest(int id);

        Task<bool> ActivateBloodRequest(int id);
        Task<bool> DeActivateBloodRequest(int id);
    }
}