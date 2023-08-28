using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAuthors.Migrations
{
    /// <inheritdoc />
    public partial class AppUser_BadPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BadPaymentHistory",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadPaymentHistory",
                table: "AspNetUsers");
        }
    }
}
