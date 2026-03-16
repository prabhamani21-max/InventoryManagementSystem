using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;


namespace InventoryManagementSystem.Service.Implementation
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _userStatusRepository;

        public StatusService(IStatusRepository userStatusRepository)
        {
            _userStatusRepository = userStatusRepository;
        }

        public async Task<List<GenericStatus>> GetAllUserStatuses()
        {
            return await _userStatusRepository.GetAllUserStatuses();
        }

        public async Task<GenericStatus> GetUserStatusById(int id)
        {
            return await _userStatusRepository.GetUserStatusById(id);
        }

        public async Task<GenericStatus> AddEditUserStatus(GenericStatus userStatus)
        {
            return await _userStatusRepository.AddEditUserStatus(userStatus);
        }

        public async Task<GenericStatus> DeleteUserStatus(int id)
        {
            return await _userStatusRepository.DeleteUserStatus(id);
        }
        public async Task<GenericStatus> GetByName(string name)
        {
            return await _userStatusRepository.GetByName(name);
        }
    }
}