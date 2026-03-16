
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IStatusRepository
    {
        Task<List<GenericStatus>> GetAllUserStatuses();
        Task<GenericStatus> GetUserStatusById(int id);
        Task<GenericStatus> AddEditUserStatus(GenericStatus userStatus);
        Task<GenericStatus> DeleteUserStatus(int id);
        Task<bool> IsUserStatusInUse(int id);
        Task<GenericStatus> GetByName(string name);
    }
}