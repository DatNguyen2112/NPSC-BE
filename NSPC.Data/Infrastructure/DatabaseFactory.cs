using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSPC.Data.Data;

namespace NSPC.Data
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly DbContext _dataContext;
        private readonly IConfiguration _configuration;

        public DatabaseFactory()
        {
            _dataContext = new SMDbContext();
        }

        public DbContext GetDbContext()
        {
            return _dataContext;
        }
    }
}