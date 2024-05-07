using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_HimuProblem_LinkWith_Distributor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DistributorId",
                table: "ProblemSet",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemSet_DistributorId",
                table: "ProblemSet",
                column: "DistributorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemSet_AspNetUsers_DistributorId",
                table: "ProblemSet",
                column: "DistributorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemSet_AspNetUsers_DistributorId",
                table: "ProblemSet");

            migrationBuilder.DropIndex(
                name: "IX_ProblemSet_DistributorId",
                table: "ProblemSet");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "ProblemSet");
        }
    }
}
