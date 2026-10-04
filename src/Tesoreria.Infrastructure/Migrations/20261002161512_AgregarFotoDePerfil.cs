using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesoreria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFotoDePerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "AspNetUsers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "AspNetUsers");
        }
    }
}
