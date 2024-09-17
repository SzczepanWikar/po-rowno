﻿// <auto-generated />
using System;
using DatabaseProjections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DatabaseProjections.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240917173957_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DatabaseProjections.Expense.ExpenseDeptorEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExpenseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ExpenseId");

                    b.HasIndex("UserId", "ExpenseId")
                        .IsUnique()
                        .HasFilter("UserId IS NOT NULL");

                    b.ToTable("ExpenseDeptorEntity");
                });

            modelBuilder.Entity("DatabaseProjections.Expense.ExpenseEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PayerId")
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

            modelBuilder.Entity("DatabaseProjections.Group.GroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("GroupEntity");
                });

            modelBuilder.Entity("DatabaseProjections.Group.UserGroupEntity", b =>
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

            modelBuilder.Entity("DatabaseProjections.User.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
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

            modelBuilder.Entity("DatabaseProjections.Expense.ExpenseDeptorEntity", b =>
                {
                    b.HasOne("DatabaseProjections.Expense.ExpenseEntity", "Expense")
                        .WithMany("Deptors")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DatabaseProjections.User.UserEntity", "User")
                        .WithMany("Depts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DatabaseProjections.Expense.ExpenseEntity", b =>
                {
                    b.HasOne("DatabaseProjections.Group.GroupEntity", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DatabaseProjections.User.UserEntity", "Payer")
                        .WithMany("Expenses")
                        .HasForeignKey("PayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Group");

                    b.Navigation("Payer");
                });

            modelBuilder.Entity("DatabaseProjections.Group.GroupEntity", b =>
                {
                    b.HasOne("DatabaseProjections.User.UserEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("DatabaseProjections.Group.UserGroupEntity", b =>
                {
                    b.HasOne("DatabaseProjections.Group.GroupEntity", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DatabaseProjections.User.UserEntity", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DatabaseProjections.Expense.ExpenseEntity", b =>
                {
                    b.Navigation("Deptors");
                });

            modelBuilder.Entity("DatabaseProjections.Group.GroupEntity", b =>
                {
                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("DatabaseProjections.User.UserEntity", b =>
                {
                    b.Navigation("Depts");

                    b.Navigation("Expenses");

                    b.Navigation("UserGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
