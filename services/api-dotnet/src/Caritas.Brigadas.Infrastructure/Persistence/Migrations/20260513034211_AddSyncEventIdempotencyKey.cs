using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncEventIdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sync_events_OrganizationId",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "sync",
                table: "sync_events",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValueSql: "CONVERT(nvarchar(36), NEWID())");

            migrationBuilder.CreateIndex(
                name: "IX_sync_events_OrganizationId_IdempotencyKey",
                schema: "sync",
                table: "sync_events",
                columns: new[] { "OrganizationId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sync_events_OrganizationId_IdempotencyKey",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.CreateIndex(
                name: "IX_sync_events_OrganizationId",
                schema: "sync",
                table: "sync_events",
                column: "OrganizationId");
        }
    }
}
