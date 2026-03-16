
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IStatusService
    {
        Task<List<GenericStatus>> GetAllUserStatuses();
        Task<GenericStatus> GetUserStatusById(int id);
        Task<GenericStatus> AddEditUserStatus(GenericStatus userStatus);
        Task<GenericStatus> DeleteUserStatus(int id);
        Task<GenericStatus> GetByName(string name);
    }
}