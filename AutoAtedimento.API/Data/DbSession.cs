using Microsoft.Data.SqlClient;
using System.Data;

namespace AutoAtedimento.API.Data
{
    public class DbSession
    {
        private readonly IConfiguration _config;

        public DbSession(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            return conn;
        }
    }
}
