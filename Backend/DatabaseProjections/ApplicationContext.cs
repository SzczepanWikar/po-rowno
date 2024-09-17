using DatabaseProjections.Expense;
using DatabaseProjections.Group;
using DatabaseProjections.User;
using Microsoft.EntityFrameworkCore;

namespace DatabaseProjections
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnUserCreating(modelBuilder);
            OnGroupCreating(modelBuilder);
            OnUserGroupCreating(modelBuilder);
            OnExpenseCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void OnUserCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().Property(e => e.Id).ValueGeneratedNever();
        }

        private void OnGroupCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupEntity>().Property(e => e.Id).ValueGeneratedNever();
        }

        private void OnUserGroupCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroupEntity>().HasKey(e => new { e.UserId, e.GroupId });

            modelBuilder
                .Entity<UserGroupEntity>()
                .HasOne(e => e.User)
                .WithMany(e => e.UserGroups)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<UserGroupEntity>()
                .HasOne(e => e.Group)
                .WithMany(e => e.UserGroups)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void OnExpenseCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseEntity>().Property(e => e.Id).ValueGeneratedNever();

            modelBuilder
                .Entity<ExpenseEntity>()
                .HasOne(e => e.Payer)
                .WithMany(e => e.Expenses)
                .HasForeignKey(e => e.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ExpenseDeptorEntity>()
                .HasIndex(e => new { e.UserId, e.ExpenseId })
                .HasFilter($"{nameof(ExpenseDeptorEntity.UserId)} IS NOT NULL")
                .IsUnique();

            modelBuilder
                .Entity<ExpenseDeptorEntity>()
                .HasOne(e => e.User)
                .WithMany(e => e.Depts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ExpenseDeptorEntity>()
                .HasOne(e => e.Expense)
                .WithMany(e => e.Deptors)
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
