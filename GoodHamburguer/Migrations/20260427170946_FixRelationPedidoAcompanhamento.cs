using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburguer.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationPedidoAcompanhamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcompanhamentoPedido_Acompanhamentos_AcompanhamentosId",
                table: "AcompanhamentoPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_AcompanhamentoPedido_Acompanhamentos_AcompanhamentosId",
                table: "AcompanhamentoPedido",
                column: "AcompanhamentosId",
                principalTable: "Acompanhamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcompanhamentoPedido_Acompanhamentos_AcompanhamentosId",
                table: "AcompanhamentoPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_AcompanhamentoPedido_Acompanhamentos_AcompanhamentosId",
                table: "AcompanhamentoPedido",
                column: "AcompanhamentosId",
                principalTable: "Acompanhamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
