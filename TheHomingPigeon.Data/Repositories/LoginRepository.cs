using Dapper;
using MySql.Data.MySqlClient;
using TheHomingPigeon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheHomingPigeon.Data.Interface;
using TheHomingPigeon.Model;

namespace TheHomingPigeon.Data.Repositories
{
    public class LoginRepository: ILoginRepository
    {
        private readonly MySqlConnection _dbConnection;

        public LoginRepository(MysqlConfiguration connectionString)
        {
            _dbConnection = new MySqlConnection(connectionString.ConnectionString);
        }
        public async Task<Login> ExistUser(string username)
        {
            var sql = @"SELECT * FROM user WHERE LOWER(username) = LOWER(@Username)";

            var parameters = new { Username = username };

            return await _dbConnection.QueryFirstOrDefaultAsync<Login>(sql, parameters);
        }

        public async Task<bool> ValidatePassword(string password, string hash)
        {

            bool verified = BCrypt.Net.BCrypt.Verify(password, hash);

            return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hash)); ;
        }
    }
}
