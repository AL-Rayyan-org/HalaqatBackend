using BCrypt.Net;
using HalaqatBackend.DTOs.Users;
using HalaqatBackend.Enums;
using HalaqatBackend.Models;
using HalaqatBackend.Repositories.Users;
using HalaqatBackend.Utils;

namespace HalaqatBackend.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllActiveUsersAsync(string? searchText, string? role)
        {
            Roles? roleEnum = null;
            
            if (!string.IsNullOrWhiteSpace(role))
            {
                var normalizedRole = role.Trim().ToLower();
                roleEnum = normalizedRole switch
                {
                    "owner" => Roles.Owner,
                    "admin" => Roles.Admin,
                    "teacher" => Roles.Teacher,
                    _ => throw new ArgumentException($"Invalid role: {role}. Allowed values are: Owner, Admin, Teacher")
                };
            }

            var users = await _userRepository.GetAllActiveUsersAsync(searchText, roleEnum);

            return users.Select(user => new UserResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = user.Role.ToString(),
                Gender = user.Gender.ToString(),
                JoinedOn = user.CreatedAt
            });
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request)
        {
            var normalizedRole = request.Role.Trim().ToLower();
            if (normalizedRole != "admin" && normalizedRole != "teacher")
            {
                throw new ArgumentException("Role must be either 'Admin' or 'Teacher'");
            }

            var normalizedGender = request.Gender.Trim().ToLower();
            if (normalizedGender != "male" && normalizedGender != "female")
            {
                throw new ArgumentException("Gender must be either 'Male' or 'Female'");
            }

            PasswordValidator.ValidateOrThrow(request.Password);

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            var roleEnum = normalizedRole == "admin" ? Roles.Admin : Roles.Teacher;
            var genderEnum = normalizedGender == "male" ? Gender.Male : Gender.Female;

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Role = roleEnum,
                Gender = genderEnum,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            return new UserResponseDto
            {
                UserId = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email ?? string.Empty,
                Role = createdUser.Role.ToString(),
                Gender = createdUser.Gender.ToString(),
                JoinedOn = createdUser.CreatedAt
            };
        }

        public async Task<UserResponseDto> ChangeUserRoleAsync(string currentUserId, string userId, string newRole)
        {
            if (currentUserId == userId)
            {
                throw new InvalidOperationException("You cannot change your own role");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Cannot change role of an inactive user");
            }

            if (user.Role == Roles.Owner)
            {
                throw new InvalidOperationException("Cannot change the role of an Owner account");
            }

            var normalizedRole = newRole.Trim().ToLower();
            var roleEnum = normalizedRole switch
            {
                "owner" => throw new InvalidOperationException("Cannot promote a user to Owner role"),
                "admin" => Roles.Admin,
                "teacher" => Roles.Teacher,
                _ => throw new ArgumentException($"Invalid role: {newRole}. Allowed values are: Admin, Teacher")
            };

            var wasTeacher = user.Role == Roles.Teacher;
            var isBecomingAdmin = roleEnum == Roles.Admin;

            if (wasTeacher && isBecomingAdmin)
            {
                await _userRepository.RemoveFromGroupTeachersAsync(userId);
            }

            await _userRepository.UpdateUserRoleAsync(userId, roleEnum);

            var updatedUser = await _userRepository.GetByIdAsync(userId);

            return new UserResponseDto
            {
                UserId = updatedUser!.Id,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email ?? string.Empty,
                Role = updatedUser.Role.ToString(),
                Gender = updatedUser.Gender.ToString(),
                JoinedOn = updatedUser.CreatedAt
            };
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (user.Role == Roles.Owner)
            {
                throw new InvalidOperationException("Cannot deactivate an Owner account");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User is already deactivated");
            }

            if (user.Role == Roles.Teacher)
            {
                await _userRepository.RemoveFromGroupTeachersAsync(userId);
            }

            return await _userRepository.DeactivateUserAsync(userId);
        }

        public async Task<bool> ChangeUserPasswordAsync(string userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            PasswordValidator.ValidateOrThrow(newPassword);

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _userRepository.UpdateUserPasswordAsync(userId, newPasswordHash);
        }
    }
}
