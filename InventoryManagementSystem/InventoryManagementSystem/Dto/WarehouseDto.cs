namespace InventoryManagementSystem.DTO
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public long? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int StatusId { get; set; }
    }
}