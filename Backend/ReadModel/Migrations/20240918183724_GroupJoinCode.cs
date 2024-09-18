using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadModel.Migrations
{
    /// <inheritdoc />
    public partial class GroupJoinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JoinCode",
                table: "GroupEntity",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinCodeValidTo",
                table: "GroupEntity",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinCode",
                table: "GroupEntity");

            migrationBuilder.DropColumn(
                name: "JoinCodeValidTo",
                table: "GroupEntity");
        }
    }
}
