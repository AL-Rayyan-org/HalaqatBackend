using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Users;
using HalaqatBackend.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Roles = "Owner,Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveUsers([FromQuery] string? searchText, [FromQuery] string? role)
        {
            try
            {
                var users = await _userService.GetAllActiveUsersAsync(searchText, role);
                return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(users));
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

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "User created successfully", 201));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpPatch("{userId}")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] ChangeUserRoleRequestDto request)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated", 401));
                }

                var user = await _userService.ChangeUserRoleAsync(currentUserId, userId, request.Role);
                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "User role changed successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            try
            {
                await _userService.DeactivateUserAsync(userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpPatch("{userId}/password")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ChangeUserPassword(string userId, [FromBody] ChangeUserPasswordRequestDto request)
        {
            try
            {
                await _userService.ChangeUserPasswordAsync(userId, request.Password);
                return Ok(ApiResponse<object>.SuccessResponse(null, "User password changed successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }
    }
}
