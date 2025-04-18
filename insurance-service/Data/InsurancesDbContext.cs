
using Microsoft.EntityFrameworkCore;
using insurance_service.Model;

namespace insurance_service.Data {
    public class InsurancesDbContext : DbContext
    {
    
        public InsurancesDbContext(DbContextOptions<InsurancesDbContext> options) : base(options)
        {
        }


        public DbSet<Insurance> Insurances { get; set; }

    }
}