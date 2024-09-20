using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitCompileMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompileMessage",
                table: "UserCommits",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompileMessage",
                table: "UserCommits");
        }
    }
}