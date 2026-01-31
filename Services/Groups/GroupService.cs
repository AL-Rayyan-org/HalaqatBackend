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
            var defaultGroup = await _groupRepository.GetDefaultGroupAsync();
            if (defaultGroup == null)
            {
                return (false, "No default group found. Cannot delete groups without a default group to transfer data.");
            }

            if (id == defaultGroup.Id)
            {
                return (false, "Cannot delete the default group.");
            }

            // Transfer students, attendance, and recitation logs to the default group
            await _groupRepository.UpdateStudentsGroupAsync(id, defaultGroup.Id);
            await _groupRepository.UpdateAttendanceGroupAsync(id, defaultGroup.Id);
            await _groupRepository.UpdateRecitationLogsGroupAsync(id, defaultGroup.Id);

            var deleted = await _groupRepository.DeleteAsync(id);
            if (!deleted)
            {
                return (false, "Failed to delete group. Group not found.");
            }

            return (true, "Group deleted successfully. Associated data transferred to default group.");
        }
    }
}