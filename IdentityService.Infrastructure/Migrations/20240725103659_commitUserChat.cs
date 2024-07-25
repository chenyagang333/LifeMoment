using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class commitUserChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_GroupsMessageToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_GroupsToUser_UserId",
                table: "User_GroupsToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogToUser_UserDialogId",
                table: "User_DialogToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogToUser_UserId",
                table: "User_DialogToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogMessage_UserDialogId",
                table: "User_DialogMessage");

            migrationBuilder.DropColumn(
                name: "UnreadCount",
                table: "User_GroupsToUser");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "User_Groups");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "User_DialogToUser");

            migrationBuilder.DropColumn(
                name: "UnreadCount",
                table: "User_DialogToUser");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                table: "User_GroupsToUser",
                newName: "DeletionTime");

            migrationBuilder.RenameColumn(
                name: "LastMessage",
                table: "User_GroupsToUser",
                newName: "UserNameWithInGroups");

            migrationBuilder.RenameColumn(
                name: "LastMessage",
                table: "User_DialogToUser",
                newName: "ToUserName");

            migrationBuilder.RenameColumn(
                name: "Received",
                table: "User_DialogMessage",
                newName: "MarkRead");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "User_GroupsToUser",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FromUserAvatar",
                table: "User_GroupsMessage",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FromUserName",
                table: "User_GroupsMessage",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "User_GroupsMessage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ToUserAvatar",
                table: "User_DialogToUser",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PostMessages",
                table: "User_DialogMessage",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "User_DialogMessage",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsToUser_UserId_UserGroupsId",
                table: "User_GroupsToUser",
                columns: new[] { "UserId", "UserGroupsId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessage_FromUserId",
                table: "User_GroupsMessage",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_ToUserId_UserId",
                table: "User_DialogToUser",
                columns: new[] { "ToUserId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_UserId_UserDialogId",
                table: "User_DialogToUser",
                columns: new[] { "UserId", "UserDialogId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogMessage_UserDialogId_ToUserId_MarkRead",
                table: "User_DialogMessage",
                columns: new[] { "UserDialogId", "ToUserId", "MarkRead" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_GroupsMessageUserDeleted");

            migrationBuilder.DropTable(
                name: "User_GroupsMessageUserUnread");

            migrationBuilder.DropIndex(
                name: "IX_User_GroupsToUser_UserId_UserGroupsId",
                table: "User_GroupsToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_GroupsMessage_FromUserId",
                table: "User_GroupsMessage");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogToUser_ToUserId_UserId",
                table: "User_DialogToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogToUser_UserId_UserDialogId",
                table: "User_DialogToUser");

            migrationBuilder.DropIndex(
                name: "IX_User_DialogMessage_UserDialogId_ToUserId_MarkRead",
                table: "User_DialogMessage");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "User_GroupsToUser");

            migrationBuilder.DropColumn(
                name: "FromUserName",
                table: "User_GroupsMessage");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "User_GroupsMessage");

            migrationBuilder.DropColumn(
                name: "ToUserAvatar",
                table: "User_DialogToUser");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "User_DialogMessage");

            migrationBuilder.RenameColumn(
                name: "UserNameWithInGroups",
                table: "User_GroupsToUser",
                newName: "LastMessage");

            migrationBuilder.RenameColumn(
                name: "DeletionTime",
                table: "User_GroupsToUser",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "ToUserName",
                table: "User_DialogToUser",
                newName: "LastMessage");

            migrationBuilder.RenameColumn(
                name: "MarkRead",
                table: "User_DialogMessage",
                newName: "Received");

            migrationBuilder.AddColumn<int>(
                name: "UnreadCount",
                table: "User_GroupsToUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "FromUserAvatar",
                table: "User_GroupsMessage",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "User_Groups",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "User_DialogToUser",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnreadCount",
                table: "User_DialogToUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "User_DialogMessage",
                keyColumn: "PostMessages",
                keyValue: null,
                column: "PostMessages",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PostMessages",
                table: "User_DialogMessage",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User_GroupsMessageToUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Received = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserGroupsMessageId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_GroupsMessageToUser", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsToUser_UserId",
                table: "User_GroupsToUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_UserDialogId",
                table: "User_DialogToUser",
                column: "UserDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogToUser_UserId",
                table: "User_DialogToUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DialogMessage_UserDialogId",
                table: "User_DialogMessage",
                column: "UserDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupsMessageToUser_UserGroupsMessageId_UserId",
                table: "User_GroupsMessageToUser",
                columns: new[] { "UserGroupsMessageId", "UserId" },
                unique: true);
        }
    }
}
