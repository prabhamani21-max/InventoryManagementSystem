using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}