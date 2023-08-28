using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAuthors.Migrations
{
    /// <inheritdoc />
    public partial class Restrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainRestriction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyId = table.Column<int>(type: "int", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainRestriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainRestriction_APIKey_KeyId",
                        column: x => x.KeyId,
                        principalTable: "APIKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IPRestriction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPRestriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IPRestriction_APIKey_KeyId",
                        column: x => x.KeyId,
                        principalTable: "APIKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainRestriction_KeyId",
                table: "DomainRestriction",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_IPRestriction_KeyId",
                table: "IPRestriction",
                column: "KeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainRestriction");

            migrationBuilder.DropTable(
                name: "IPRestriction");
        }
    }
}
