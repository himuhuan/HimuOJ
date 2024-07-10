using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himu.EntityFramework.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserChatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriend_AspNetUsers_FriendId",
                table: "UserFriend");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriend_AspNetUsers_UserId",
                table: "UserFriend");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFriend",
                table: "UserFriend");

            migrationBuilder.RenameTable(
                name: "UserFriend",
                newName: "UserFriends");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriend_FriendId",
                table: "UserFriends",
                newName: "IX_UserFriends_FriendId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFriends",
                table: "UserFriends",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.CreateTable(
                name: "UserChatSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FriendId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChatSessions_AspNetUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserChatSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "varchar(3000)", maxLength: 3000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SendTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SessionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SenderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_UserChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "UserChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatSessions_FriendId",
                table: "UserChatSessions",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatSessions_UserId",
                table: "UserChatSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_FriendId",
                table: "UserFriends",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_UserId",
                table: "UserFriends",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_FriendId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_UserId",
                table: "UserFriends");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "UserChatSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFriends",
                table: "UserFriends");

            migrationBuilder.RenameTable(
                name: "UserFriends",
                newName: "UserFriend");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriends_FriendId",
                table: "UserFriend",
                newName: "IX_UserFriend_FriendId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFriend",
                table: "UserFriend",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriend_AspNetUsers_FriendId",
                table: "UserFriend",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriend_AspNetUsers_UserId",
                table: "UserFriend",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
