
using Microsoft.EntityFrameworkCore;
using web_api_example.Model;

namespace web_api_example.Data {
    public class PersonsDbContext : DbContext
    {
    
        public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }

    }
}