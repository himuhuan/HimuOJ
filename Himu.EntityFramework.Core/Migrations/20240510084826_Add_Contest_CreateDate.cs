using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_Contest_CreateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contests_Information_DistributeDateTime",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Information_DistributeDateTime",
                table: "Contests");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreateDate",
                table: "Contests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Contests");

            migrationBuilder.AddColumn<DateTime>(
                name: "Information_DistributeDateTime",
                table: "Contests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Information_DistributeDateTime",
                table: "Contests",
                column: "Information_DistributeDateTime");
        }
    }
}