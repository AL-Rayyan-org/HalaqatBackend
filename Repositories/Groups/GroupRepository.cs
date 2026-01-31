using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;
using HalaqatBackend.Utils;

namespace HalaqatBackend.Repositories.Groups
{
    public class GroupRepository : IGroupRepository
    {
        private readonly DapperContext _context;

        public GroupRepository(DapperContext context)
        {
            _context=context;
        }


        public async Task<Group?> GetAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM groups";
            return await connection.QueryFirstOrDefaultAsync<Group>(sql);
        }

        public async Task<Group?> GetByIdAsync(string id)
        {
            using var connection= _context.CreateConnection();
            var sql="SELECT * FROM groups WHERE id= @Id";
            return await connection.QueryFirstOrDefaultAsync<Group>(sql,new{Id = id});
        }

        public async Task<Group> CreateAsync(Group group)
        {
            using var connection=_context.CreateConnection();
            var sql=@"INSERT INTO groups(id,name,recitation_days,is_default,members_limit,created_at)
                      VALUES (@Id,@Name,@RecitationDays,@IsDefault,@MembersLimit,@CreatedAt)RETURNING *";

            var parameters = new
            {
                group.Id,
                group.Name,
                group.RecitationDays,
                group.IsDefault,
                group.MembersLimit,
                group.CreatedAt
            };

            var createdGroup = await connection.QuerySingleAsync<Group>(sql, parameters);
            return createdGroup;
        }

        public async Task<Group> UpdateAsync(Group group)
        {
            using var connection=_context.CreateConnection();
            var sql=@"UPDATE groups
                     SET name = @Name,
                        recitation_days = @RecitationDays,
                        is_default = @IsDefault,
                        members_limit = @MembersLimit
                        WHERE id=@Id RETURNING *";
            var updateGroup=await connection.QuerySingleAsync<Group>(sql,group);
            return updateGroup;
        }

        public async Task<bool> HasUsersAsync(string groupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM students WHERE groupId = @GroupId";
            var count = await connection.QuerySingleAsync<int>(sql, new { GroupId = groupId });
            return count > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection=_context.CreateConnection();
            var sql="DELETE FROM groups WHERE id=@Id";
            var affectedRows=await connection.ExecuteAsync(sql,new{Id=id});
            return affectedRows>0;
        }

        public async Task<Group?> GetDefaultGroupAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM groups WHERE is_default = TRUE LIMIT 1";
            return await connection.QueryFirstOrDefaultAsync<Group>(sql);
        }

        public async Task<IEnumerable<string>> GetTeacherGroupIdsAsync(string teacherId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT group_id FROM group_teachers WHERE teacher_id = @TeacherId";
            return await connection.QueryAsync<string>(sql, new { TeacherId = teacherId });
        }

        public async Task<bool> IsTeacherInGroupAsync(string teacherId, string groupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(1) FROM group_teachers WHERE teacher_id = @TeacherId AND group_id = @GroupId";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TeacherId = teacherId, GroupId = groupId });
            return count > 0;
        }

        public async Task<bool> UpdateStudentsGroupAsync(string oldGroupId, string newGroupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE students SET group_id = @NewGroupId WHERE group_id = @OldGroupId";
            var affectedRows = await connection.ExecuteAsync(sql, new { OldGroupId = oldGroupId, NewGroupId = newGroupId });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateAttendanceGroupAsync(string oldGroupId, string newGroupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE attendance SET group_id = @NewGroupId WHERE group_id = @OldGroupId";
            var affectedRows = await connection.ExecuteAsync(sql, new { OldGroupId = oldGroupId, NewGroupId = newGroupId });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateRecitationLogsGroupAsync(string oldGroupId, string newGroupId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE recitation_logs SET group_id = @NewGroupId WHERE group_id = @OldGroupId";
            var affectedRows = await connection.ExecuteAsync(sql, new { OldGroupId = oldGroupId, NewGroupId = newGroupId });
            return affectedRows > 0;
        }
    }
}