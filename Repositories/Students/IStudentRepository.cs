using HalaqatBackend.Enums;
using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Students
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(string studentId);
        Task<Student?> GetByUserIdAsync(string userId);
        Task<Student> CreateAsync(Student student);
        Task<bool> UpdateGroupAsync(string studentId, string groupId);
        Task<IEnumerable<StudentWithUser>> GetAllStudentsAsync(string? searchText, string? groupId);
        Task<IEnumerable<StudentWithUser>> GetStudentsByGroupIdsAsync(IEnumerable<string> groupIds, string? searchText, string? groupId);
        Task<StudentWithUser?> GetStudentDetailsAsync(string studentId);
        Task<bool> UpdateRecitationLogsGroupAsync(string studentId, string newGroupId);
    }

    public class StudentWithUser
    {
        public string StudentId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Gender Gender { get; set; }
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? Info { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
