using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Profile;
using HalaqatBackend.Services.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(
                        "User not authenticated", 401));
                }

                var result = await _profileService.GetUserProfileAsync(userId);
                return Ok(ApiResponse<ProfileResponseDto>.SuccessResponse(
                    result, "Profile retrieved successfully", 200));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Invalid input data", 400));
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(
                        "User not authenticated", 401));
                }

                var result = await _profileService.UpdateUserProfileAsync(userId, request);
                return Ok(ApiResponse<ProfileResponseDto>.SuccessResponse(
                    result, "Profile updated successfully", 200));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Invalid input data", 400));
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(
                        "User not authenticated", 401));
                }

                var result = await _profileService.ChangePasswordAsync(userId, request);
                
                
                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(
                        null, "Password changed successfully.", 200));
                }

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Failed to change password", 400));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message, 401));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }
    }
}
