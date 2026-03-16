namespace InventoryManagementSystem.DTO
{
    public class UserKycDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? PanCardNumber { get; set; }
        public string? AadhaarCardNumber { get; set; }
        public bool IsVerified { get; set; }
        public int StatusId { get; set; }
    }
}