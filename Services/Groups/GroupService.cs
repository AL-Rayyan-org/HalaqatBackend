using HalaqatBackend.Repositories.Groups;

namespace HalaqatBackend.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<(bool success, string message)> DeleteAsync(string id)
        {
            var hasUsers = await _groupRepository.HasUsersAsync(id);
            if (hasUsers)
            {
                // Try to migrate to default group instead of blocking deletion
                var defaultGroup = await _groupRepository.GetDefaultGroupAsync();
                if (defaultGroup == null)
                {
                    return (false, "Cannot delete group with students. Please remove all students first or set a default group.");
                }

                // Migrate data from deleted group to default group
                var migrationSuccess = await _groupRepository.MigrateDeletedGroupDataAsync(id, defaultGroup.Id);
                if (!migrationSuccess)
                {
                    return (false, "Failed to migrate group data to default group.");
                }
            }

            var deleted = await _groupRepository.DeleteAsync(id);
            if (!deleted)
            {
                return (false, "Failed to delete group. Group not found.");
            }

            return (true, "Group deleted successfully.");
        }

        public async Task<(bool success, string message)> SetDefaultGroupAsync(string groupId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return (false, "Group not found.");
            }

            var updated = await _groupRepository.SetDefaultGroupAsync(groupId);
            if (!updated)
            {
                return (false, "Failed to set default group.");
            }

            return (true, "Default group set successfully.");
        }
    }
}