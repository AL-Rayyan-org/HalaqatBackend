using BCrypt.Net;
using HalaqatBackend.DTOs.Students;
using HalaqatBackend.Enums;
using HalaqatBackend.Models;
using HalaqatBackend.Repositories.Groups;
using HalaqatBackend.Repositories.Students;
using HalaqatBackend.Repositories.Users;
using HalaqatBackend.Utils;
using System.Data;
using Dapper;
using HalaqatBackend.Data;

namespace HalaqatBackend.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly DapperContext _context;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            DapperContext context)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _context = context;
        }

        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentRequestDto request)
        {
            // Validate gender
            var normalizedGender = request.Gender.Trim().ToLower();
            if (normalizedGender != "male" && normalizedGender != "female")
            {
                throw new ArgumentException("Gender must be either 'Male' or 'Female'");
            }

            // Validate password
            PasswordValidator.ValidateOrThrow(request.Password);

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Determine the group ID
            string groupId;
            if (string.IsNullOrWhiteSpace(request.GroupId))
            {
                // Get default group
                var defaultGroup = await _groupRepository.GetDefaultGroupAsync();
                if (defaultGroup == null)
                {
                    throw new InvalidOperationException("Cannot add student without a group. Default group does not exist. Please create a default group first.");
                }
                groupId = defaultGroup.Id;
            }
            else
            {
                // Verify the specified group exists
                var group = await _groupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    throw new ArgumentException($"Group with ID '{request.GroupId}' does not exist");
                }
                groupId = request.GroupId;
            }

            var genderEnum = normalizedGender == "male" ? Gender.Male : Gender.Female;

            // Use transaction to ensure atomicity
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Create user
                var userId = Guid.NewGuid().ToString();
                var userSql = @"INSERT INTO users (id, first_name, last_name, email, password_hash, phone, is_active, role, gender, created_at)
                               VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @Phone, @IsActive, @RoleString, @GenderString, @CreatedAt)
                               RETURNING *";

                var userParams = new
                {
                    Id = userId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Phone = request.Phone,
                    IsActive = true,
                    RoleString = Roles.Student.ToString(),
                    GenderString = genderEnum.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await connection.QuerySingleAsync<User>(userSql, userParams, transaction);

                // Create student
                var studentId = Guid.NewGuid().ToString();
                var studentSql = @"INSERT INTO students (id, user_id, group_id, info)
                                  VALUES (@Id, @UserId, @GroupId, @Info)
                                  RETURNING *";

                var studentParams = new
                {
                    Id = studentId,
                    UserId = userId,
                    GroupId = groupId,
                    Info = request.Info
                };

                var createdStudent = await connection.QuerySingleAsync<Student>(studentSql, studentParams, transaction);

                // Get group name for response
                var groupSql = "SELECT name FROM groups WHERE id = @GroupId";
                var groupName = await connection.QueryFirstOrDefaultAsync<string>(groupSql, new { GroupId = groupId }, transaction) ?? string.Empty;

                transaction.Commit();

                return new StudentResponseDto
                {
                    StudentId = createdStudent.Id,
                    UserId = createdUser.Id,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Email = createdUser.Email ?? string.Empty,
                    Phone = createdUser.Phone,
                    Gender = createdUser.Gender.ToString(),
                    GroupId = createdStudent.GroupId,
                    GroupName = groupName,
                    Info = createdStudent.Info,
                    JoinedOn = createdUser.CreatedAt
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(string currentUserId, string currentUserRole, string? searchText, string? groupId)
        {
            IEnumerable<StudentWithUser> students;

            if (currentUserRole == "Teacher")
            {
                // Teachers can only see students in their assigned groups
                var teacherGroupIds = await _groupRepository.GetTeacherGroupIdsAsync(currentUserId);
                students = await _studentRepository.GetStudentsByGroupIdsAsync(teacherGroupIds, searchText, groupId);
            }
            else
            {
                // Admins and Owners can see all students
                students = await _studentRepository.GetAllStudentsAsync(searchText, groupId);
            }

            return students.Select(s => new StudentResponseDto
            {
                StudentId = s.StudentId,
                UserId = s.UserId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                Phone = s.Phone,
                Gender = s.Gender.ToString(),
                GroupId = s.GroupId,
                GroupName = s.GroupName,
                Info = s.Info,
                JoinedOn = s.CreatedAt
            });
        }

        public async Task<StudentResponseDto> GetStudentByIdAsync(string studentId, string currentUserId, string currentUserRole)
        {
            var student = await _studentRepository.GetStudentDetailsAsync(studentId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found");
            }

            // If teacher, verify they have access to this student's group
            if (currentUserRole == "Teacher")
            {
                var isTeacherInGroup = await _groupRepository.IsTeacherInGroupAsync(currentUserId, student.GroupId);
                if (!isTeacherInGroup)
                {
                    throw new UnauthorizedAccessException("You do not have access to this student");
                }
            }

            return new StudentResponseDto
            {
                StudentId = student.StudentId,
                UserId = student.UserId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Phone = student.Phone,
                Gender = student.Gender.ToString(),
                GroupId = student.GroupId,
                GroupName = student.GroupName,
                Info = student.Info,
                JoinedOn = student.CreatedAt
            };
        }

        public async Task<bool> ChangeStudentPasswordAsync(string studentId, string newPassword)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found");
            }

            var user = await _userRepository.GetByIdAsync(student.UserId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Cannot change password for an inactive student");
            }

            PasswordValidator.ValidateOrThrow(newPassword);

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _userRepository.UpdateUserPasswordAsync(student.UserId, newPasswordHash);
        }

        public async Task<bool> DeactivateStudentAsync(string studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found");
            }

            var user = await _userRepository.GetByIdAsync(student.UserId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Student is already deactivated");
            }

            // Get default group
            var defaultGroup = await _groupRepository.GetDefaultGroupAsync();
            if (defaultGroup == null)
            {
                throw new InvalidOperationException("Cannot deactivate student. Default group does not exist.");
            }

            // Use transaction to ensure atomicity
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Deactivate user
                var deactivateSql = "UPDATE users SET is_active = false WHERE id = @UserId";
                await connection.ExecuteAsync(deactivateSql, new { UserId = student.UserId }, transaction);

                // Move student to default group
                var updateStudentGroupSql = "UPDATE students SET group_id = @GroupId WHERE id = @StudentId";
                await connection.ExecuteAsync(updateStudentGroupSql, new { StudentId = studentId, GroupId = defaultGroup.Id }, transaction);

                // Update recitation_logs group_id
                var updateLogsSql = "UPDATE recitation_logs SET group_id = @GroupId WHERE student_id = @StudentId";
                await connection.ExecuteAsync(updateLogsSql, new { StudentId = studentId, GroupId = defaultGroup.Id }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<StudentResponseDto> TransferStudentAsync(string studentId, string targetGroupId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student == null)
            {
                throw new InvalidOperationException("Student not found");
            }

            var user = await _userRepository.GetByIdAsync(student.UserId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Cannot transfer an inactive student");
            }

            // Verify target group exists
            var targetGroup = await _groupRepository.GetByIdAsync(targetGroupId);
            if (targetGroup == null)
            {
                throw new ArgumentException($"Target group with ID '{targetGroupId}' does not exist");
            }

            if (student.GroupId == targetGroupId)
            {
                throw new InvalidOperationException("Student is already in the target group");
            }

            // Use transaction to ensure atomicity
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Update student's group
                var updateStudentSql = "UPDATE students SET group_id = @GroupId WHERE id = @StudentId";
                await connection.ExecuteAsync(updateStudentSql, new { StudentId = studentId, GroupId = targetGroupId }, transaction);

                // Update recitation_logs group_id
                var updateLogsSql = "UPDATE recitation_logs SET group_id = @GroupId WHERE student_id = @StudentId";
                await connection.ExecuteAsync(updateLogsSql, new { StudentId = studentId, GroupId = targetGroupId }, transaction);

                transaction.Commit();

                return new StudentResponseDto
                {
                    StudentId = student.Id,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Phone = user.Phone,
                    Gender = user.Gender.ToString(),
                    GroupId = targetGroupId,
                    GroupName = targetGroup.Name,
                    Info = student.Info,
                    JoinedOn = user.CreatedAt
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
