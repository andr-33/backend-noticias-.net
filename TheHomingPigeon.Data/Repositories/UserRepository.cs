using TheHomingPigeon.Data.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using TheHomingPigeon.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace TheHomingPigeon.Data.Interface
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _dbConnection;

        public UserRepository(MysqlConfiguration connectionString)
        {
            _dbConnection = new MySqlConnection(connectionString.ConnectionString);
        }

        public async Task<IEnumerable<UserResponse>> GetActiveUsers()
        {
            var sql = @"SELECT iduser, username, email FROM user";

            return await _dbConnection.QueryAsync<UserResponse>(sql, new { });
        }

        public async Task<bool> InsertUser(User user)
        {
            var hasPassword = BCrypt.Net.BCrypt.HashPassword(user.password);

            var sql = @"INSERT INTO user (username, email, password) VALUES (@Username, @Email ,@Password)";

            var parameters = new {Username = user.username, Email = user.email, Password = hasPassword};

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, parameters);

            return rowsAffected > 0;
        }

        public async Task<bool> ValidateExistUser(string username, string email)
        {
            var sql = @"SELECT * FROM user WHERE LOWER(username) = LOWER(@Username) OR LOWER(email) = LOWER(@Email)";

            var parameters = new { Username = username, Email = email};

            var result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql,parameters);

            return result > 0;
        }
    }
}
