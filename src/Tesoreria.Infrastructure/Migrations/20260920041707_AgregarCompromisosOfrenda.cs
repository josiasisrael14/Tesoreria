using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tesoreria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCompromisosOfrenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompromisosOfrenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfrendaEspecialId = table.Column<int>(type: "int", nullable: false),
                    MiembroId = table.Column<int>(type: "int", nullable: false),
                    MontoComprometido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisosOfrenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromisosOfrenda_Miembros_MiembroId",
                        column: x => x.MiembroId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompromisosOfrenda_OfrendasEspeciales_OfrendaEspecialId",
                        column: x => x.OfrendaEspecialId,
                        principalTable: "OfrendasEspeciales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosOfrenda_MiembroId",
                table: "CompromisosOfrenda",
                column: "MiembroId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosOfrenda_OfrendaEspecialId_MiembroId",
                table: "CompromisosOfrenda",
                columns: new[] { "OfrendaEspecialId", "MiembroId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompromisosOfrenda");
        }
    }
}
