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
                return (false, "Cannot delete group with student. Please remove all students first.");
            }

            var deleted = await _groupRepository.DeleteAsync(id);
            if (!deleted)
            {
                return (false, "Failed to delete group. Group not found.");
            }

            return (true, "Group deleted successfully.");
        }
    }
}