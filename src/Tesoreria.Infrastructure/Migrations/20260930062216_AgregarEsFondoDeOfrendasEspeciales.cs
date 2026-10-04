using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesoreria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsFondoDeOfrendasEspeciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsFondoDeOfrendasEspeciales",
                table: "Fondos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsFondoDeOfrendasEspeciales",
                table: "Fondos");
        }
    }
}
