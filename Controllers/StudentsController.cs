using HalaqatBackend.DTOs;
using HalaqatBackend.DTOs.Students;
using HalaqatBackend.Services.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        [Authorize(Roles = "Owner,Admin,Teacher")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequestDto request)
        {
            try
            {
                var student = await _studentService.CreateStudentAsync(request);
                return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(student, "Student created successfully", 201));
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

        [HttpGet]
        [Authorize(Roles = "Owner,Admin,Teacher")]
        public async Task<IActionResult> GetAllStudents([FromQuery] string? searchText, [FromQuery] string? groupId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserRole))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated", 401));
                }

                var students = await _studentService.GetAllStudentsAsync(currentUserId, currentUserRole, searchText, groupId);
                return Ok(ApiResponse<IEnumerable<StudentResponseDto>>.SuccessResponse(students));
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

        [HttpGet("{studentId}")]
        [Authorize(Roles = "Owner,Admin,Teacher")]
        public async Task<IActionResult> GetStudentById(string studentId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserRole))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated", 401));
                }

                var student = await _studentService.GetStudentByIdAsync(studentId, currentUserId, currentUserRole);
                return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(student));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message, 403));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpPatch("password")]
        [Authorize(Roles = "Owner,Admin,Teacher")]
        public async Task<IActionResult> ChangeStudentPassword([FromBody] ChangeStudentPasswordRequestDto request)
        {
            try
            {
                await _studentService.ChangeStudentPasswordAsync(request.StudentId, request.Password);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Student password changed successfully"));
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

        [HttpDelete("{studentId}")]
        [Authorize(Roles = "Owner,Admin,Teacher")]
        public async Task<IActionResult> DeactivateStudent(string studentId)
        {
            try
            {
                await _studentService.DeactivateStudentAsync(studentId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Student deactivated successfully"));
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

        [HttpPatch("transfer")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> TransferStudent([FromBody] TransferStudentRequestDto request)
        {
            try
            {
                var student = await _studentService.TransferStudentAsync(request.StudentId, request.TargetGroupId);
                return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(student, "Student transferred successfully"));
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
