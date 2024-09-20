using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_Indexs_ForAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompilerInformation_CompilerName",
                table: "UserCommits",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Detail_Title",
                table: "ProblemSet",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Information_Title",
                table: "Contests",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommits_CompilerInformation_CompilerName",
                table: "UserCommits",
                column: "CompilerInformation_CompilerName");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommits_Status",
                table: "UserCommits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSet_Detail_Title",
                table: "ProblemSet",
                column: "Detail_Title")
                .Annotation("MySql:IndexPrefixLength", new[] { 10 });

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Information_DistributeDateTime",
                table: "Contests",
                column: "Information_DistributeDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Information_Title",
                table: "Contests",
                column: "Information_Title")
                .Annotation("MySql:IndexPrefixLength", new[] { 50 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCommits_CompilerInformation_CompilerName",
                table: "UserCommits");

            migrationBuilder.DropIndex(
                name: "IX_UserCommits_Status",
                table: "UserCommits");

            migrationBuilder.DropIndex(
                name: "IX_ProblemSet_Detail_Title",
                table: "ProblemSet");

            migrationBuilder.DropIndex(
                name: "IX_Contests_Information_DistributeDateTime",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_Information_Title",
                table: "Contests");

            migrationBuilder.AlterColumn<string>(
                name: "CompilerInformation_CompilerName",
                table: "UserCommits",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Detail_Title",
                table: "ProblemSet",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Information_Title",
                table: "Contests",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}