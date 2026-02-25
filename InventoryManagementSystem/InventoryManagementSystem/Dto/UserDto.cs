namespace InventoryManagementSystem.DTO
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
          public string? RoleName { get; set; }
        public int StatusId { get; set; }
          public string? StatusName { get; set; }
        public int Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string? Address { get; set; }
        public string ContactNumber { get; set; }
        public string? ProfileImage { get; set; }

    }
}
