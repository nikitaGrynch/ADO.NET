using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADO_NET.Migrations
{
    /// <inheritdoc />
    public partial class ManagerProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagerProduct",
                columns: table => new
                {
                    ManagersId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerProduct", x => new { x.ManagersId, x.ProductsId });
                    
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ManagerId",
                table: "Sales",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ProductId",
                table: "Sales",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_IdMainDep",
                table: "Managers",
                column: "IdMainDep");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_IdSecDep",
                table: "Managers",
                column: "IdSecDep");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerProduct_ProductsId",
                table: "ManagerProduct",
                column: "ProductsId");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "ManagerProduct");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ManagerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ProductId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Managers_IdMainDep",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_IdSecDep",
                table: "Managers");
        }
    }
}
