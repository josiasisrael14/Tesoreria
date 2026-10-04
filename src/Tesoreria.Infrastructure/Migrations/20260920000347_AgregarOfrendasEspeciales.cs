using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesoreria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOfrendasEspeciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfrendaEspecialId",
                table: "Transacciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OfrendasEspeciales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaMonto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfrendasEspeciales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_OfrendaEspecialId",
                table: "Transacciones",
                column: "OfrendaEspecialId");

            migrationBuilder.CreateIndex(
                name: "IX_OfrendasEspeciales_Nombre",
                table: "OfrendasEspeciales",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_OfrendasEspeciales_OfrendaEspecialId",
                table: "Transacciones",
                column: "OfrendaEspecialId",
                principalTable: "OfrendasEspeciales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_OfrendasEspeciales_OfrendaEspecialId",
                table: "Transacciones");

            migrationBuilder.DropTable(
                name: "OfrendasEspeciales");

            migrationBuilder.DropIndex(
                name: "IX_Transacciones_OfrendaEspecialId",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "OfrendaEspecialId",
                table: "Transacciones");
        }
    }
}
