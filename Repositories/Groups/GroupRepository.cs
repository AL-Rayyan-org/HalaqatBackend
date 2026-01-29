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
            var sql=@"INSERT INTO groups(id,name,recitationDays,members_limit,is_default,createdAt)
                      VALUES (@Id,@Name,@RecitationDays,@MembersLimit,@IsDefault,@CreatedAt)RETURNING *";

            var parameters = new
            {
                group.Id,
                group.Name,
                group.RecitationDays,
                group.MembersLimit,
                group.IsDefault,
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
                        recitationDays = @RecitationDays,
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
            var sql = "SELECT * FROM groups WHERE is_default = true LIMIT 1";
            return await connection.QueryFirstOrDefaultAsync<Group>(sql);
        }

        public async Task<bool> SetDefaultGroupAsync(string groupId)
        {
            using var connection = _context.CreateConnection();
            
            // First, remove is_default from all other groups
            var resetSql = "UPDATE groups SET is_default = false WHERE is_default = true";
            await connection.ExecuteAsync(resetSql);
            
            // Then set the new default group
            var setSql = "UPDATE groups SET is_default = true WHERE id = @Id";
            var affectedRows = await connection.ExecuteAsync(setSql, new { Id = groupId });
            
            return affectedRows > 0;
        }

        public async Task<bool> MigrateDeletedGroupDataAsync(string sourceGroupId, string defaultGroupId)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Migrate students
                var migrateStudentsSql = @"UPDATE students 
                                           SET group_id = @DefaultGroupId 
                                           WHERE group_id = @SourceGroupId";
                await connection.ExecuteAsync(migrateStudentsSql, 
                    new { DefaultGroupId = defaultGroupId, SourceGroupId = sourceGroupId }, transaction);

                // Migrate attendance records
                var migrateAttendanceSql = @"UPDATE attendance 
                                            SET group_id = @DefaultGroupId 
                                            WHERE group_id = @SourceGroupId";
                await connection.ExecuteAsync(migrateAttendanceSql, 
                    new { DefaultGroupId = defaultGroupId, SourceGroupId = sourceGroupId }, transaction);

                // Migrate recitation logs
                var migrateRecitationSql = @"UPDATE recitation_logs 
                                            SET group_id = @DefaultGroupId 
                                            WHERE group_id = @SourceGroupId";
                await connection.ExecuteAsync(migrateRecitationSql, 
                    new { DefaultGroupId = defaultGroupId, SourceGroupId = sourceGroupId }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            }
    }
}