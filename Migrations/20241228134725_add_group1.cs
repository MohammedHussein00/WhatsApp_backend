using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Aditor.Migrations
{
    /// <inheritdoc />
    public partial class add_group1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_AspNetUsers_UserId",
                table: "GroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUser_Group_GroupId",
                table: "GroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageGroup_Group_GroupId",
                table: "MessageGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageGroup_Messages_MessageId",
                table: "MessageGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageGroup",
                table: "MessageGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.RenameTable(
                name: "MessageGroup",
                newName: "MessageGroups");

            migrationBuilder.RenameTable(
                name: "GroupUser",
                newName: "GroupUsers");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_MessageGroup_GroupId",
                table: "MessageGroups",
                newName: "IX_MessageGroups_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupUser_GroupId",
                table: "GroupUsers",
                newName: "IX_GroupUsers_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageGroups",
                table: "MessageGroups",
                columns: new[] { "MessageId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupUsers",
                table: "GroupUsers",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_AspNetUsers_UserId",
                table: "GroupUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageGroups_Groups_GroupId",
                table: "MessageGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageGroups_Messages_MessageId",
                table: "MessageGroups",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_AspNetUsers_UserId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupUsers_Groups_GroupId",
                table: "GroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageGroups_Groups_GroupId",
                table: "MessageGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageGroups_Messages_MessageId",
                table: "MessageGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageGroups",
                table: "MessageGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupUsers",
                table: "GroupUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "MessageGroups",
                newName: "MessageGroup");

            migrationBuilder.RenameTable(
                name: "GroupUsers",
                newName: "GroupUser");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameIndex(
                name: "IX_MessageGroups_GroupId",
                table: "MessageGroup",
                newName: "IX_MessageGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupUsers_GroupId",
                table: "GroupUser",
                newName: "IX_GroupUser_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageGroup",
                table: "MessageGroup",
                columns: new[] { "MessageId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupUser",
                table: "GroupUser",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_AspNetUsers_UserId",
                table: "GroupUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupUser_Group_GroupId",
                table: "GroupUser",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageGroup_Group_GroupId",
                table: "MessageGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageGroup_Messages_MessageId",
                table: "MessageGroup",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
