using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesoreria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAnioAOfrendasEspeciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OfrendasEspeciales_Nombre",
                table: "OfrendasEspeciales");

            migrationBuilder.AddColumn<int>(
                name: "Anio",
                table: "OfrendasEspeciales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OfrendasEspeciales_Nombre_Anio",
                table: "OfrendasEspeciales",
                columns: new[] { "Nombre", "Anio" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OfrendasEspeciales_Nombre_Anio",
                table: "OfrendasEspeciales");

            migrationBuilder.DropColumn(
                name: "Anio",
                table: "OfrendasEspeciales");

            migrationBuilder.CreateIndex(
                name: "IX_OfrendasEspeciales_Nombre",
                table: "OfrendasEspeciales",
                column: "Nombre",
                unique: true);
        }
    }
}
