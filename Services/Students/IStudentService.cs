using HalaqatBackend.DTOs.Students;

namespace HalaqatBackend.Services.Students
{
    public interface IStudentService
    {
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentRequestDto request);
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(string currentUserId, string currentUserRole, string? searchText, string? groupId);
        Task<StudentResponseDto> GetStudentByIdAsync(string studentId, string currentUserId, string currentUserRole);
        Task<bool> ChangeStudentPasswordAsync(string studentId, string newPassword);
        Task<bool> DeactivateStudentAsync(string studentId);
        Task<StudentResponseDto> TransferStudentAsync(string studentId, string targetGroupId);
    }
}
