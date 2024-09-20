using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserCommitCaches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AcceptedCommitCount",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AcceptedProblemCount",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalCommitCount",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ContestProblemCode",
                columns: table => new
                {
                    ProblemId = table.Column<long>(type: "bigint", nullable: false),
                    ContestId = table.Column<long>(type: "bigint", nullable: false),
                    ProblemCode = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestProblemCode", x => new { x.ProblemId, x.ContestId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                UPDATE AspNetUsers
                SET
                    TotalCommitCount = (
                        SELECT COUNT(*)
                        FROM UserCommits
                        WHERE UserCommits.UserId = AspNetUsers.Id
                    ),
                    AcceptedProblemCount = (
                        SELECT COUNT(DISTINCT ProblemId)
                        FROM UserCommits
                        WHERE UserCommits.UserId = AspNetUsers.Id
                        AND UserCommits.Status = 'ACCEPTED'
                    ),
                    AcceptedCommitCount = (
                        SELECT COUNT(*)
                        FROM UserCommits
                        WHERE UserCommits.UserId = AspNetUsers.Id
                        AND UserCommits.Status = 'ACCEPTED'
                    )
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestProblemCode");

            migrationBuilder.DropColumn(
                name: "AcceptedCommitCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AcceptedProblemCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalCommitCount",
                table: "AspNetUsers");
        }
    }
}
