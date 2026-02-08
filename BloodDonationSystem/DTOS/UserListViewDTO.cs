using BloodDonationSystem.Models;

namespace BloodDonationSystem.DTOS
{
    public class UserListViewDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string BloodType { get; set; }
        public List<string> Roles { get; set; }      
        public List<int> RoleIds { get; set; }
    }
}
