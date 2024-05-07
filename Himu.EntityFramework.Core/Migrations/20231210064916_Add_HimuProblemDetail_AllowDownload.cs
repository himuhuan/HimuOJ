using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_HimuProblemDetail_AllowDownload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Detail_AllowDownloadAnswer",
                table: "ProblemSet",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Detail_AllowDownloadInput",
                table: "ProblemSet",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Detail_AllowDownloadAnswer",
                table: "ProblemSet");

            migrationBuilder.DropColumn(
                name: "Detail_AllowDownloadInput",
                table: "ProblemSet");
        }
    }
}
