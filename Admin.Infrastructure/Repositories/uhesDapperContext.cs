using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Admin.Infrastructure.Repositories
{
   public class uhesDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public uhesDapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
