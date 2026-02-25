namespace InventoryManagementSystem.DTO
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string TANNumber { get; set; }
   
        public string GSTNumber { get; set; }
        public int StatusId { get; set; }
    }
}