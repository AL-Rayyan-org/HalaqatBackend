
namespace HalaqatBackend.Services.Groups
{
    public interface IGroupService
    {
        Task<(bool success, string message)> DeleteAsync(string id);
        Task<(bool success, string message)> SetDefaultGroupAsync(string groupId);
    }
    
}