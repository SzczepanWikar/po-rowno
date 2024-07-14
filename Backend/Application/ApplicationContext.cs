using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Core.Foo.Foo> Foos { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }
    }
}
