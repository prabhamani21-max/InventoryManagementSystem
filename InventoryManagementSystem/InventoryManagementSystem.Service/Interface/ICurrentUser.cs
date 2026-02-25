namespace InventoryManagementSystem.Service.Interface
{
    public interface ICurrentUser
    {
        public long UserId { get; set; }
        int RoleId { get; set; }
    }
}
