using BloodDonationSystem.Models;

namespace BloodDonationSystem.DTOS
{
    public class UserListViewDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string BloodType { get; set; }

        public int bloodTypeId { get; set; }

        public List<string> Roles { get; set; }
        
        public List<int> RoleIds { get; set; }

        public bool isUserAdoner { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int? UserAge { get; set; }

        public DateTime? UserCreationDate { get; set; }

        public bool isUserAvailableToDonate { get; set; }

    }
}
