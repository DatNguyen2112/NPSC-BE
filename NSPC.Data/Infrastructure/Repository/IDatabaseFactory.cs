using Microsoft.EntityFrameworkCore;

namespace NSPC.Data
{
    public interface IDatabaseFactory
    {
        DbContext GetDbContext();
    }
}