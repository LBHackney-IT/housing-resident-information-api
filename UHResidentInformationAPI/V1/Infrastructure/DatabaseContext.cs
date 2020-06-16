using UHResidentInformationAPI.V1.Domain;
using UHResidentInformationAPI.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UHResidentInformationAPI.V1.Infrastructure
{

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DatabaseEntity> DatabaseEntities { get; set; }
    }
}
