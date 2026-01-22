using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Auth;
using HalaqatBackend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[HttpPost("register")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ApiResponse<object>.ErrorResponse(
        //                "Invalid input data", 400));
        //        }

        //        var result = await _authService.RegisterAsync(request);
        //        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
        //            result, "Registration successful", 201));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, 400));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ApiResponse<object>.ErrorResponse(
        //            ex.ToString(), 500));
        //    }
        //}

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Invalid input data", 400));
                }

                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                    result, "Login successful", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message, 401));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
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

                var result = await _authService.ChangePasswordAsync(userId, request);
                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(
                        null, "Password changed successfully. Please login again.", 200));
                }

                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Failed to change password", 400));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message, 401));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var role = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(
                        "User not authenticated", 401));
                }

                var userData = new
                {
                    UserId = userId,
                    Email = email,
                    Role = role
                };

                return Ok(ApiResponse<object>.SuccessResponse(
                    userData, "User data retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    ex.ToString(), 500));
            }
        }
    }
}
