using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueryModel.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "GroupEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinCodeValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupEntity_UserEntity_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "BalanceEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeptorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(
                        type: "decimal(13,4)",
                        precision: 13,
                        scale: 4,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceEntity_GroupEntity_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_BalanceEntity_UserEntity_DeptorId",
                        column: x => x.DeptorId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_BalanceEntity_UserEntity_PayerId",
                        column: x => x.PayerId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExpenseEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(
                        type: "decimal(13,4)",
                        precision: 13,
                        scale: 4,
                        nullable: false
                    ),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseEntity_GroupEntity_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ExpenseEntity_UserEntity_PayerId",
                        column: x => x.PayerId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "UserGroupEntity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupEntity", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroupEntity_GroupEntity_GroupId",
                        column: x => x.GroupId,
                        principalTable: "GroupEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserGroupEntity_UserEntity_UserId",
                        column: x => x.UserId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ExpenseDeptorEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(
                        type: "decimal(13,4)",
                        precision: 13,
                        scale: 4,
                        nullable: false
                    ),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseDeptorEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseDeptorEntity_ExpenseEntity_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "ExpenseEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ExpenseDeptorEntity_UserEntity_UserId",
                        column: x => x.UserId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_BalanceEntity_DeptorId",
                table: "BalanceEntity",
                column: "DeptorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_BalanceEntity_GroupId",
                table: "BalanceEntity",
                column: "GroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_BalanceEntity_PayerId_GroupId_DeptorId",
                table: "BalanceEntity",
                columns: new[] { "PayerId", "GroupId", "DeptorId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseDeptorEntity_ExpenseId",
                table: "ExpenseDeptorEntity",
                column: "ExpenseId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseDeptorEntity_UserId_ExpenseId",
                table: "ExpenseDeptorEntity",
                columns: new[] { "UserId", "ExpenseId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntity_GroupId",
                table: "ExpenseEntity",
                column: "GroupId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntity_PayerId",
                table: "ExpenseEntity",
                column: "PayerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GroupEntity_OwnerId",
                table: "GroupEntity",
                column: "OwnerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupEntity_GroupId",
                table: "UserGroupEntity",
                column: "GroupId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BalanceEntity");

            migrationBuilder.DropTable(name: "ExpenseDeptorEntity");

            migrationBuilder.DropTable(name: "UserGroupEntity");

            migrationBuilder.DropTable(name: "ExpenseEntity");

            migrationBuilder.DropTable(name: "GroupEntity");

            migrationBuilder.DropTable(name: "UserEntity");
        }
    }
}
