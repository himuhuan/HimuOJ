using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProblemAcceptedCount",
                table: "ProblemSet",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProblemCommitCount",
                table: "ProblemSet",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProblemAcceptedCount",
                table: "ProblemSet");

            migrationBuilder.DropColumn(
                name: "ProblemCommitCount",
                table: "ProblemSet");
        }
    }
}
