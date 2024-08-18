using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_Dialog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Dialog", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_DialogMessage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserDialogId = table.Column<long>(type: "bigint", nullable: false),
                    FromUserId = table.Column<long>(type: "bigint", nullable: false),
                    ToUserId = table.Column<long>(type: "bigint", nullable: false),
                    PostMessages = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarkRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RetractMessage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FromUser_Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ToUser_Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_DialogMessage", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_DialogToUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserDialogId = table.Column<long>(type: "bigint", nullable: false),
                    ToUserId = table.Column<long>(type: "bigint", nullable: false),
                    ToUserName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ToUserAvatar = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_DialogToUser", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_Groups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: false),
                    Icon = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notice = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Introduce = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Groups", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_GroupsMessage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserGroupsId = table.Column<long>(type: "bigint", nullable: false),
                    FromUserId = table.Column<long>(type: "bigint", nullable: false),
                    FromUserName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FromUserAvatar = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PostMessages = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RetractMessage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_GroupsMessage", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_GroupsMessageUserDeleted",
                columns: table => new
                {
                    UserGroupsId = table.Column<long>(type: "bigint", nullable: false),
                    ToUserId = table.Column<long>(type: "bigint", nullable: false),
                    UserGroupsMessageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_GroupsMessageUserDeleted", x => new { x.UserGroupsId, x.ToUserId, x.UserGroupsMessageId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_GroupsMessageUserUnread",
                columns: table => new
                {
                    UserGroupsId = table.Column<long>(type: "bigint", nullable: false),
                    ToUserId = table.Column<long>(type: "bigint", nullable: false),
                    UserGroupsMessageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_GroupsMessageUserUnread", x => new { x.UserGroupsId, x.ToUserId, x.UserGroupsMessageId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_GroupsToUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserGroupsId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameWithInGroups = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_GroupsToUser", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogMessage_UserDialogId_ToUserId_MarkRead",
                table: "User_DialogMessage",
                columns: new[] { "UserDialogId", "ToUserId", "MarkRead" });

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_ToUserId_UserId",
                table: "User_DialogToUser",
                columns: new[] { "ToUserId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_UserId_UserDialogId",
                table: "User_DialogToUser",
                columns: new[] { "UserId", "UserDialogId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessage_FromUserId",
                table: "User_GroupsMessage",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessage_UserGroupsId",
                table: "User_GroupsMessage",
                column: "UserGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessageUserDeleted_UserGroupsId_ToUserId_UserGrou~",
                table: "User_GroupsMessageUserDeleted",
                columns: new[] { "UserGroupsId", "ToUserId", "UserGroupsMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessageUserUnread_UserGroupsId_ToUserId_UserGroup~",
                table: "User_GroupsMessageUserUnread",
                columns: new[] { "UserGroupsId", "ToUserId", "UserGroupsMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsToUser_UserGroupsId",
                table: "User_GroupsToUser",
                column: "UserGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsToUser_UserId_UserGroupsId",
                table: "User_GroupsToUser",
                columns: new[] { "UserId", "UserGroupsId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Dialog");

            migrationBuilder.DropTable(
                name: "User_DialogMessage");

            migrationBuilder.DropTable(
                name: "User_DialogToUser");

            migrationBuilder.DropTable(
                name: "User_Groups");

            migrationBuilder.DropTable(
                name: "User_GroupsMessage");

            migrationBuilder.DropTable(
                name: "User_GroupsMessageUserDeleted");

            migrationBuilder.DropTable(
                name: "User_GroupsMessageUserUnread");

            migrationBuilder.DropTable(
                name: "User_GroupsToUser");
        }
    }
}
