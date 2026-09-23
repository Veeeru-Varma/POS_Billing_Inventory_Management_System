using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace POS.DAL.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext( IConfiguration configuration)
        {
            _configuration= configuration;
        }

        public DbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("MyConnection"));
        }
    }
}
