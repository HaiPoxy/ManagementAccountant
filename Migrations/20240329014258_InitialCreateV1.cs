using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagermnet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsReceivedNotes",
                columns: table => new
                {
                    GRNId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentDay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersonID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceivedNotes", x => x.GRNId);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedNotes_Persons_PersonID",
                        column: x => x.PersonID,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceivedNoteDetails",
                columns: table => new
                {
                    GRNDId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehousId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPirce = table.Column<int>(type: "int", nullable: false),
                    DebitAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GRN_Id = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceivedNoteDetails", x => x.GRNDId);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedNoteDetails_GoodsReceivedNotes_GRN_Id",
                        column: x => x.GRN_Id,
                        principalTable: "GoodsReceivedNotes",
                        principalColumn: "GRNId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceivedNoteDetails_ProductCategories_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedNoteDetails_GRN_Id",
                table: "GoodsReceivedNoteDetails",
                column: "GRN_Id");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedNoteDetails_ProductId",
                table: "GoodsReceivedNoteDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceivedNotes_PersonID",
                table: "GoodsReceivedNotes",
                column: "PersonID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceivedNoteDetails");

            migrationBuilder.DropTable(
                name: "GoodsReceivedNotes");
        }
    }
}
