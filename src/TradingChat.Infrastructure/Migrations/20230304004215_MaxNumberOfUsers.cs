using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MaxNumberOfUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfUsers",
                schema: "tc",
                table: "chat_rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxNumberOfUsers",
                schema: "tc",
                table: "chat_rooms");
        }
    }
}
