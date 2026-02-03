namespace BloodDonationSystem.DTOS
{

    public class BloodTypeSelectionDTO
    {
        public int BloodTypeId { get; set; }
        public string BloodTypeName { get; set; }
        public bool IsSelected { get; set; } // The Checkbox
        public int Quantity { get; set; }    // The Quantity Input
    }


}
