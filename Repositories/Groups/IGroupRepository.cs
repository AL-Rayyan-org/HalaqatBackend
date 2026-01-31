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
        Task<IEnumerable<string>> GetTeacherGroupIdsAsync(string teacherId);
        Task<bool> IsTeacherInGroupAsync(string teacherId, string groupId);
        Task<bool> UpdateStudentsGroupAsync(string oldGroupId, string newGroupId);
        Task<bool> UpdateAttendanceGroupAsync(string oldGroupId, string newGroupId);
        Task<bool> UpdateRecitationLogsGroupAsync(string oldGroupId, string newGroupId);
    }
}
