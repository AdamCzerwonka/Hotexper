using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotexper.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateRoomItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_RoomItem_RoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomItem_Rooms_RoomId",
                table: "RoomItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomItem",
                table: "RoomItem");

            migrationBuilder.RenameTable(
                name: "RoomItem",
                newName: "RoomItems");

            migrationBuilder.RenameIndex(
                name: "IX_RoomItem_RoomId",
                table: "RoomItems",
                newName: "IX_RoomItems_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomItems",
                table: "RoomItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_RoomItems_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "RoomItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomItems_Rooms_RoomId",
                table: "RoomItems",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_RoomItems_RoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomItems_Rooms_RoomId",
                table: "RoomItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomItems",
                table: "RoomItems");

            migrationBuilder.RenameTable(
                name: "RoomItems",
                newName: "RoomItem");

            migrationBuilder.RenameIndex(
                name: "IX_RoomItems_RoomId",
                table: "RoomItem",
                newName: "IX_RoomItem_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomItem",
                table: "RoomItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_RoomItem_RoomId",
                table: "Reservations",
                column: "RoomId",
                principalTable: "RoomItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomItem_Rooms_RoomId",
                table: "RoomItem",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
