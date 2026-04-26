using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburguer.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDbContextAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Sanduiches_SanduicheId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Sanduiches_SanduicheId",
                table: "Pedidos",
                column: "SanduicheId",
                principalTable: "Sanduiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Sanduiches_SanduicheId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Sanduiches_SanduicheId",
                table: "Pedidos",
                column: "SanduicheId",
                principalTable: "Sanduiches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
