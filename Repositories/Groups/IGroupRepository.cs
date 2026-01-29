using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Groups
{
        public interface IGroupRepository
    {
        Task<Group?> GetAsync();
        Task<Group?> GetByIdAsync(string id);
        Task<Group> CreateAsync(Group group);
        Task<Group> UpdateAsync(Group group);
        Task<bool> DeleteAsync(string id);
        Task<bool> HasUsersAsync(string groupId);
        Task<Group?> GetDefaultGroupAsync();
        Task<bool> SetDefaultGroupAsync(string groupId);
        Task<bool> MigrateDeletedGroupDataAsync(string sourceGroupId, string defaultGroupId);
    }

}
