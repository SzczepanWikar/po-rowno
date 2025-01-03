﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QueryModel;

#nullable disable

namespace QueryModel.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ReadModel.Expense.BalanceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasPrecision(13, 4)
                        .HasColumnType("decimal(13,4)");

                    b.Property<Guid>("DeptorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PayerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DeptorId");

                    b.HasIndex("GroupId");

                    b.HasIndex("PayerId", "GroupId", "DeptorId")
                        .IsUnique();

                    b.ToTable("BalanceEntity");
                });

            modelBuilder.Entity("ReadModel.Expense.ExpenseDeptorEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(13, 4)
                        .HasColumnType("decimal(13,4)");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ExpenseId");

                    b.HasIndex("UserId", "ExpenseId")
                        .IsUnique();

                    b.ToTable("ExpenseDeptorEntity");
                });

            modelBuilder.Entity("ReadModel.Expense.ExpenseEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(13, 4)
                        .HasColumnType("decimal(13,4)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PaymentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PayerId");

                    b.ToTable("ExpenseEntity");
                });

            modelBuilder.Entity("ReadModel.Group.GroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JoinCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("JoinCodeValidTo")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("GroupEntity");
                });

            modelBuilder.Entity("ReadModel.User.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserEntity");
                });

            modelBuilder.Entity("ReadModel.UserGroup.UserGroupEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroupEntity");
                });

            modelBuilder.Entity("ReadModel.Expense.BalanceEntity", b =>
                {
                    b.HasOne("ReadModel.User.UserEntity", "Deptor")
                        .WithMany("DeptBalances")
                        .HasForeignKey("DeptorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ReadModel.Group.GroupEntity", "Group")
                        .WithMany("Balances")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReadModel.User.UserEntity", "Payer")
                        .WithMany("CreditBalances")
                        .HasForeignKey("PayerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Deptor");

                    b.Navigation("Group");

                    b.Navigation("Payer");
                });

            modelBuilder.Entity("ReadModel.Expense.ExpenseDeptorEntity", b =>
                {
                    b.HasOne("ReadModel.Expense.ExpenseEntity", "Expense")
                        .WithMany("Deptors")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReadModel.User.UserEntity", "User")
                        .WithMany("Depts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ReadModel.Expense.ExpenseEntity", b =>
                {
                    b.HasOne("ReadModel.Group.GroupEntity", "Group")
                        .WithMany("Expenses")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReadModel.User.UserEntity", "Payer")
                        .WithMany("Expenses")
                        .HasForeignKey("PayerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Payer");
                });

            modelBuilder.Entity("ReadModel.Group.GroupEntity", b =>
                {
                    b.HasOne("ReadModel.User.UserEntity", "Owner")
                        .WithMany("OwnedGroups")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("ReadModel.UserGroup.UserGroupEntity", b =>
                {
                    b.HasOne("ReadModel.Group.GroupEntity", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReadModel.User.UserEntity", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ReadModel.Expense.ExpenseEntity", b =>
                {
                    b.Navigation("Deptors");
                });

            modelBuilder.Entity("ReadModel.Group.GroupEntity", b =>
                {
                    b.Navigation("Balances");

                    b.Navigation("Expenses");

                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("ReadModel.User.UserEntity", b =>
                {
                    b.Navigation("CreditBalances");

                    b.Navigation("DeptBalances");

                    b.Navigation("Depts");

                    b.Navigation("Expenses");

                    b.Navigation("OwnedGroups");

                    b.Navigation("UserGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
