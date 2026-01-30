using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;
using HalaqatBackend.Utils;

namespace HalaqatBackend.Repositories.Students
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DapperContext _context;

        public StudentRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(string studentId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM students WHERE id = @StudentId";
            return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { StudentId = studentId });
        }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM students WHERE user_id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { UserId = userId });
        }

        public async Task<Student> CreateAsync(Student student)
        {
            using var connection = _context.CreateConnection();
            var sql = @"INSERT INTO students (id, user_id, group_id, info)
                       VALUES (@Id, @UserId, @GroupId, @Info)
                       RETURNING *";

            var parameters = new
            {
                student.Id,
                student.UserId,
                student.GroupId,
                student.Info
            };

            return await connection.QuerySingleAsync<Student>(sql, parameters);
        }

        public async Task<bool> UpdateGroupAsync(string studentId, string groupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE students SET group_id = @GroupId WHERE id = @StudentId";
            var affectedRows = await connection.ExecuteAsync(sql, new { StudentId = studentId, GroupId = groupId });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<StudentWithUser>> GetAllStudentsAsync(string? searchText, string? groupId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new Dictionary<string, object>();
            var whereClauses = new List<string> { "u.is_active = true", "u.role = 'Student'" };

            searchText = SearchHelper.NormalizeSearchText(searchText);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchClause = SearchHelper.BuildSearchClause(
                    searchText,
                    new[] { "u.first_name", "u.last_name", "u.email" },
                    parameters
                );
                whereClauses.Add(searchClause);
            }

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                whereClauses.Add("s.group_id = @GroupId");
                parameters["GroupId"] = groupId;
            }

            var whereClause = string.Join(" AND ", whereClauses);
            var sql = $@"SELECT 
                            s.id AS StudentId,
                            s.user_id AS UserId,
                            u.first_name AS FirstName,
                            u.last_name AS LastName,
                            u.email AS Email,
                            u.phone AS Phone,
                            u.gender AS Gender,
                            s.group_id AS GroupId,
                            g.name AS GroupName,
                            s.info AS Info,
                            u.created_at AS CreatedAt
                        FROM students s
                        INNER JOIN users u ON s.user_id = u.id
                        LEFT JOIN groups g ON s.group_id = g.id
                        WHERE {whereClause}
                        ORDER BY u.created_at DESC";

            return await connection.QueryAsync<StudentWithUser>(sql, parameters);
        }

        public async Task<IEnumerable<StudentWithUser>> GetStudentsByGroupIdsAsync(IEnumerable<string> groupIds, string? searchText, string? groupId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new Dictionary<string, object>();
            var whereClauses = new List<string> { "u.is_active = true", "u.role = 'Student'" };

            var groupIdsList = groupIds.ToList();
            if (groupIdsList.Count > 0)
            {
                whereClauses.Add("s.group_id = ANY(@GroupIds)");
                parameters["GroupIds"] = groupIdsList.ToArray();
            }
            else
            {
                // Teacher has no assigned groups, return empty
                return Enumerable.Empty<StudentWithUser>();
            }

            searchText = SearchHelper.NormalizeSearchText(searchText);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchClause = SearchHelper.BuildSearchClause(
                    searchText,
                    new[] { "u.first_name", "u.last_name", "u.email" },
                    parameters
                );
                whereClauses.Add(searchClause);
            }

            // If specific groupId is provided, filter further (must be in teacher's groups)
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                if (!groupIdsList.Contains(groupId))
                {
                    // Requested group is not in teacher's groups
                    return Enumerable.Empty<StudentWithUser>();
                }
                whereClauses.Add("s.group_id = @SpecificGroupId");
                parameters["SpecificGroupId"] = groupId;
            }

            var whereClause = string.Join(" AND ", whereClauses);
            var sql = $@"SELECT 
                            s.id AS StudentId,
                            s.user_id AS UserId,
                            u.first_name AS FirstName,
                            u.last_name AS LastName,
                            u.email AS Email,
                            u.phone AS Phone,
                            u.gender AS Gender,
                            s.group_id AS GroupId,
                            g.name AS GroupName,
                            s.info AS Info,
                            u.created_at AS CreatedAt
                        FROM students s
                        INNER JOIN users u ON s.user_id = u.id
                        LEFT JOIN groups g ON s.group_id = g.id
                        WHERE {whereClause}
                        ORDER BY u.created_at DESC";

            return await connection.QueryAsync<StudentWithUser>(sql, parameters);
        }

        public async Task<StudentWithUser?> GetStudentDetailsAsync(string studentId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT 
                            s.id AS StudentId,
                            s.user_id AS UserId,
                            u.first_name AS FirstName,
                            u.last_name AS LastName,
                            u.email AS Email,
                            u.phone AS Phone,
                            u.gender AS Gender,
                            s.group_id AS GroupId,
                            g.name AS GroupName,
                            s.info AS Info,
                            u.created_at AS CreatedAt
                        FROM students s
                        INNER JOIN users u ON s.user_id = u.id
                        LEFT JOIN groups g ON s.group_id = g.id
                        WHERE s.id = @StudentId AND u.is_active = true";

            return await connection.QueryFirstOrDefaultAsync<StudentWithUser>(sql, new { StudentId = studentId });
        }

        public async Task<bool> UpdateRecitationLogsGroupAsync(string studentId, string newGroupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE recitation_logs SET group_id = @NewGroupId WHERE student_id = @StudentId";
            await connection.ExecuteAsync(sql, new { StudentId = studentId, NewGroupId = newGroupId });
            return true;
        }
    }
}
