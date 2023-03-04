using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_messages",
                schema: "tc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChatUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatRoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_rooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalSchema: "tc",
                        principalTable: "chat_rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_users_ChatUserId",
                        column: x => x.ChatUserId,
                        principalSchema: "tc",
                        principalTable: "chat_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ChatRoomId",
                schema: "tc",
                table: "chat_messages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ChatUserId",
                schema: "tc",
                table: "chat_messages",
                column: "ChatUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_messages",
                schema: "tc");
        }
    }
}
