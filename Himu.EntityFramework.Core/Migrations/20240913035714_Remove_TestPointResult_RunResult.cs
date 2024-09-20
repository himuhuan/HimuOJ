using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Remove_TestPointResult_RunResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunResult_ExitCode",
                table: "PointResults");

            migrationBuilder.DropColumn(
                name: "RunResult_Message",
                table: "PointResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RunResult_ExitCode",
                table: "PointResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RunResult_Message",
                table: "PointResults",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
