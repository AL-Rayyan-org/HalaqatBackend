using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Auth;
using HalaqatBackend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
