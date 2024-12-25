using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackseCommerce_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Popular = table.Column<bool>(type: "bit", nullable: false),
                    BestSeller = table.Column<bool>(type: "bit", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "lanches1.png", "Lanches" },
                    { 2, "combos1.png", "Combos" },
                    { 3, "naturais1.png", "Naturais" },
                    { 4, "refrigerantes1.png", "Bebidas" },
                    { 5, "sucos1.png", "Sucos" },
                    { 6, "sobremesas1.png", "Sobremesas" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Available", "BestSeller", "CategoryId", "Details", "ImageUrl", "Name", "Popular", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, true, true, 1, "Pão fofinho, hambúrger de carne bovina temperada, cebola, mostarda e ketchup ", "hamburger1.jpeg", "Hamburger padrão", true, 15m, 13 },
                    { 2, true, false, 1, "Pão fofinho, hambúrguer de carne bovina temperada e queijo por todos os lados.", "hamburger3.jpeg", "CheeseBurger padrão", true, 18m, 10 },
                    { 3, true, false, 1, "Pão fofinho, hambúrger de carne bovina temperada, cebola,alface, mostarda e ketchup ", "hamburger4.jpeg", "CheeseSalada padrão", false, 19m, 13 },
                    { 4, false, false, 2, "Pão fofinho, hambúrguer de carne bovina temperada e queijo, refrigerante e fritas", "combo1.jpeg", "Hambúrger, batata fritas, refrigerante ", true, 25m, 10 },
                    { 5, true, false, 2, "Pão fofinho, hambúrguer de carne bovina ,refrigerante e fritas, cebola, maionese e ketchup", "combo2.jpeg", "CheeseBurger, batata fritas , refrigerante", false, 27m, 13 },
                    { 6, true, false, 2, "Pão fofinho, hambúrguer de carne bovina ,refrigerante e fritas, cebola, maionese e ketchup", "combo3.jpeg", "CheeseSalada, batata fritas, refrigerante", true, 28m, 10 },
                    { 7, true, false, 3, "Pão integral com folhas e tomate", "lanche_natural1.jpeg", "Lanche Natural com folhas", false, 14m, 13 },
                    { 8, true, false, 3, "Pão integral, folhas, tomate e queijo.", "lanche_natural2.jpeg", "Lanche Natural e queijo", true, 15m, 10 },
                    { 9, true, false, 3, "Lanche vegano com ingredientes saudáveis", "lanche_vegano1.jpeg", "Lanche Vegano", false, 25m, 18 },
                    { 10, true, false, 4, "Refrigerante Coca Cola", "coca_cola1.jpeg", "Coca-Cola", true, 21m, 7 },
                    { 11, true, false, 4, "Refrigerante de Guaraná", "guarana1.jpeg", "Guaraná", false, 25m, 6 },
                    { 12, true, false, 4, "Refrigerante Pepsi Cola", "pepsi1.jpeg", "Pepsi", false, 21m, 6 },
                    { 13, true, false, 5, "Suco de laranja saboroso e nutritivo", "suco_laranja.jpeg", "Suco de laranja", false, 11m, 10 },
                    { 14, true, false, 5, "Suco de morango fresquinhos", "suco_morango1.jpeg", "Suco de morango", false, 15m, 13 },
                    { 15, true, false, 5, "Suco de uva natural sem acúcar feito com a fruta", "suco_uva1.jpeg", "Suco de uva", false, 13m, 10 },
                    { 16, true, false, 4, "Água mineral natural fresquinha", "agua_mineral1.jpeg", "Água", false, 5m, 10 },
                    { 17, true, false, 6, "Cookies de Chocolate com pedaços de chocolate", "cookie1.jpeg", "Cookies de chocolate", true, 8m, 10 },
                    { 18, true, true, 6, "Cookies de baunilha saborosos e crocantes", "cookie2.jpeg", "Cookies de Baunilha", false, 8m, 13 },
                    { 19, true, false, 6, "Torta suíca com creme e camadas de doce de leite", "torta_suica1.jpeg", "Torta Suíca", true, 10m, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ProductId",
                table: "ShoppingCartItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ShoppingCartItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
