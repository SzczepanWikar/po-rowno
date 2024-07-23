using Core.Foo;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public class ApplicationContext : DbContext
    {
        public DbSet<FooEntity> Foos { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            onFooCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        void onFooCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FooEntity>().Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
