using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Aditor.Migrations
{
    /// <inheritdoc />
    public partial class add_group2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Groups");
        }
    }
}
