using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChatOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                schema: "tc",
                table: "chat_rooms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_chat_rooms_OwnerId",
                schema: "tc",
                table: "chat_rooms",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_chat_rooms_chat_users_OwnerId",
                schema: "tc",
                table: "chat_rooms",
                column: "OwnerId",
                principalSchema: "tc",
                principalTable: "chat_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_rooms_chat_users_OwnerId",
                schema: "tc",
                table: "chat_rooms");

            migrationBuilder.DropIndex(
                name: "IX_chat_rooms_OwnerId",
                schema: "tc",
                table: "chat_rooms");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "tc",
                table: "chat_rooms");
        }
    }
}
