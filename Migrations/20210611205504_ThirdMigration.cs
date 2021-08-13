using Microsoft.EntityFrameworkCore.Migrations;

namespace BAG_Site.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bagitem_Art_ArtId",
                table: "Bagitem");

            migrationBuilder.DropForeignKey(
                name: "FK_Bagitem_Bag_BagId",
                table: "Bagitem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bagitem",
                table: "Bagitem");

            migrationBuilder.RenameTable(
                name: "Bagitem",
                newName: "Bagitems");

            migrationBuilder.RenameIndex(
                name: "IX_Bagitem_BagId",
                table: "Bagitems",
                newName: "IX_Bagitems_BagId");

            migrationBuilder.RenameIndex(
                name: "IX_Bagitem_ArtId",
                table: "Bagitems",
                newName: "IX_Bagitems_ArtId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bagitems",
                table: "Bagitems",
                column: "BagitemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bagitems_Art_ArtId",
                table: "Bagitems",
                column: "ArtId",
                principalTable: "Art",
                principalColumn: "ArtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bagitems_Bag_BagId",
                table: "Bagitems",
                column: "BagId",
                principalTable: "Bag",
                principalColumn: "BagId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bagitems_Art_ArtId",
                table: "Bagitems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bagitems_Bag_BagId",
                table: "Bagitems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bagitems",
                table: "Bagitems");

            migrationBuilder.RenameTable(
                name: "Bagitems",
                newName: "Bagitem");

            migrationBuilder.RenameIndex(
                name: "IX_Bagitems_BagId",
                table: "Bagitem",
                newName: "IX_Bagitem_BagId");

            migrationBuilder.RenameIndex(
                name: "IX_Bagitems_ArtId",
                table: "Bagitem",
                newName: "IX_Bagitem_ArtId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bagitem",
                table: "Bagitem",
                column: "BagitemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bagitem_Art_ArtId",
                table: "Bagitem",
                column: "ArtId",
                principalTable: "Art",
                principalColumn: "ArtId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bagitem_Bag_BagId",
                table: "Bagitem",
                column: "BagId",
                principalTable: "Bag",
                principalColumn: "BagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
