using Microsoft.EntityFrameworkCore;
using QueryModel.Expense;
using QueryModel.Group;
using QueryModel.User;
using QueryModel.UserGroup;

namespace QueryModel
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
            OnBalanceCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<decimal>().HavePrecision(13, 4);
        }

        private void OnUserCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().Property(e => e.Id).ValueGeneratedNever();
        }

        private void OnGroupCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupEntity>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder
                .Entity<GroupEntity>()
                .HasOne(e => e.Owner)
                .WithMany(e => e.OwnedGroups)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void OnUserGroupCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroupEntity>().HasKey(e => new { e.UserId, e.GroupId });

            modelBuilder
                .Entity<UserGroupEntity>()
                .HasOne(e => e.User)
                .WithMany(e => e.UserGroups)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<UserGroupEntity>()
                .HasOne(e => e.Group)
                .WithMany(e => e.UserGroups)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
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
                .IsUnique();

            modelBuilder
                .Entity<ExpenseDeptorEntity>()
                .HasOne(e => e.User)
                .WithMany(e => e.Depts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<ExpenseDeptorEntity>()
                .HasOne(e => e.Expense)
                .WithMany(e => e.Deptors)
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<ExpenseEntity>()
                .HasOne(e => e.Group)
                .WithMany(e => e.Expenses)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void OnBalanceCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<BalanceEntity>()
                .HasIndex(e => new
                {
                    e.PayerId,
                    e.GroupId,
                    e.DeptorId,
                })
                .IsUnique();

            modelBuilder
                .Entity<BalanceEntity>()
                .HasOne(e => e.Group)
                .WithMany(e => e.Balances)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<BalanceEntity>()
                .HasOne(e => e.Payer)
                .WithMany(e => e.CreditBalances)
                .HasForeignKey(e => e.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<BalanceEntity>()
                .HasOne(e => e.Deptor)
                .WithMany(e => e.DeptBalances)
                .HasForeignKey(e => e.DeptorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
