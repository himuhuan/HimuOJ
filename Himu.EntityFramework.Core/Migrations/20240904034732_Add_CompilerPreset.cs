using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_CompilerPreset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompilerPresets",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupportedExtensions = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Command = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Shared = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Timeout = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompilerPresets", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Add the default compiler preset
            migrationBuilder.Sql(
                "INSERT INTO `CompilerPresets` (`Name`, `Language`, `SupportedExtensions`, `Command`, `Shared`, `Timeout`) " +
                "VALUES ('GCC', 'C', '.c', 'gcc -o {0} {1}', 0, '00:00:10')");

            migrationBuilder.RenameColumn(
                name: "CompilerInformation_MessageFromCompiler",
                table: "UserCommits",
                newName: "MessageFromCompiler");

            migrationBuilder.RenameColumn(
                name: "CompilerInformation_CompilerName",
                table: "UserCommits",
                newName: "CompilerName");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommits_CompilerInformation_CompilerName",
                table: "UserCommits",
                newName: "IX_UserCommits_CompilerName");

            migrationBuilder.UpdateData(
                table: "UserCommits",
                keyColumn: "MessageFromCompiler",
                keyValue: null,
                column: "MessageFromCompiler",
                value: "GCC");

            migrationBuilder.AlterColumn<string>(
                name: "MessageFromCompiler",
                table: "UserCommits",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(4096)",
                oldMaxLength: 4096,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CompilerName",
                table: "UserCommits",
                type: "varchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommits_CompilerPresets_CompilerName",
                table: "UserCommits",
                column: "CompilerName",
                principalTable: "CompilerPresets",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommits_CompilerPresets_CompilerName",
                table: "UserCommits");

            migrationBuilder.DropTable(
                name: "CompilerPresets");

            migrationBuilder.RenameColumn(
                name: "MessageFromCompiler",
                table: "UserCommits",
                newName: "CompilerInformation_MessageFromCompiler");

            migrationBuilder.RenameColumn(
                name: "CompilerName",
                table: "UserCommits",
                newName: "CompilerInformation_CompilerName");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommits_CompilerName",
                table: "UserCommits",
                newName: "IX_UserCommits_CompilerInformation_CompilerName");

            migrationBuilder.AlterColumn<string>(
                name: "CompilerInformation_MessageFromCompiler",
                table: "UserCommits",
                type: "varchar(4096)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CompilerInformation_CompilerName",
                table: "UserCommits",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
