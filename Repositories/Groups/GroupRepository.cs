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
            var sql=@"INSERT INTO groups(id,name,recitationDays,createdAt)
                      VALUES (@Id,@Name,@RecitationDays,@CreatedAt)RETURNING *";

            var parameters = new
            {
                group.Id,
                group.Name,
                group.RecitationDays,
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
                        recitationDays = @RecitationDays
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

    }
}