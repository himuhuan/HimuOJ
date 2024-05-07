using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_TestPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPointResult_TestPoints_TestPointId",
                table: "TestPointResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TestPointResult_UserCommits_CommitId",
                table: "TestPointResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestPointResult",
                table: "TestPointResult");

            migrationBuilder.RenameTable(
                name: "TestPointResult",
                newName: "PointResults");

            migrationBuilder.RenameIndex(
                name: "IX_TestPointResult_TestPointId",
                table: "PointResults",
                newName: "IX_PointResults_TestPointId");

            migrationBuilder.RenameIndex(
                name: "IX_TestPointResult_CommitId",
                table: "PointResults",
                newName: "IX_PointResults_CommitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointResults",
                table: "PointResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointResults_TestPoints_TestPointId",
                table: "PointResults",
                column: "TestPointId",
                principalTable: "TestPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointResults_UserCommits_CommitId",
                table: "PointResults",
                column: "CommitId",
                principalTable: "UserCommits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointResults_TestPoints_TestPointId",
                table: "PointResults");

            migrationBuilder.DropForeignKey(
                name: "FK_PointResults_UserCommits_CommitId",
                table: "PointResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointResults",
                table: "PointResults");

            migrationBuilder.RenameTable(
                name: "PointResults",
                newName: "TestPointResult");

            migrationBuilder.RenameIndex(
                name: "IX_PointResults_TestPointId",
                table: "TestPointResult",
                newName: "IX_TestPointResult_TestPointId");

            migrationBuilder.RenameIndex(
                name: "IX_PointResults_CommitId",
                table: "TestPointResult",
                newName: "IX_TestPointResult_CommitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestPointResult",
                table: "TestPointResult",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPointResult_TestPoints_TestPointId",
                table: "TestPointResult",
                column: "TestPointId",
                principalTable: "TestPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestPointResult_UserCommits_CommitId",
                table: "TestPointResult",
                column: "CommitId",
                principalTable: "UserCommits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
