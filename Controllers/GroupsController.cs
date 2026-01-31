using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Groups;
using HalaqatBackend.Repositories.Groups;
using HalaqatBackend.Services.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("groups")]
    [Authorize(Roles = "Owner,Admin,Teacher")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupService _groupService;

        public GroupsController(IGroupRepository groupRepository, IGroupService groupService)
        {
            _groupRepository = groupRepository;
            _groupService = groupService;
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(string groupId)
        {
            try
            {
                var group = await _groupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Group not found", 404));
                }
                return Ok(ApiResponse<dynamic>.SuccessResponse(group, "Group retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequestDto request)
        {
            try
            {
                var group = new HalaqatBackend.Models.Group
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    RecitationDays = request.RecitationDays,
                    IsDefault = request.IsDefault,
                    MembersLimit = request.MembersLimit,
                    CreatedAt = DateTime.UtcNow
                };

                var createdGroup = await _groupRepository.CreateAsync(group);
                return StatusCode(200, ApiResponse<dynamic>.SuccessResponse(createdGroup, "Group created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpPatch("{groupId}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> UpdateGroup(string groupId, [FromBody] UpdateGroupRequestDto request)
        {
            try
            {
                var group = await _groupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Group not found", 404));
                }

                group.Name = request.Name ?? group.Name;
                group.RecitationDays = request.RecitationDays ?? group.RecitationDays;

                var updatedGroup = await _groupRepository.UpdateAsync(group);
                return Ok(ApiResponse<dynamic>.SuccessResponse(updatedGroup, "Group updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpDelete("{groupId}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> DeleteGroup(string groupId)
        {
            try
            {
                var (success, message) = await _groupService.DeleteAsync(groupId);
                
                if (!success)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(message, 400));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }
    }
}
