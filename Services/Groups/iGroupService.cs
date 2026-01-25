
namespace HalaqatBackend.Services.Groups
{
    public interface IGroupService
    {
        Task<(bool success, string message)> DeleteAsync(string id);
    }
}
