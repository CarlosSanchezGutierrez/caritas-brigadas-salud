using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientOfflineSourceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceBrigadeId",
                schema: "clinical",
                table: "patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalPatientId",
                schema: "clinical",
                table: "patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientOperationId",
                schema: "clinical",
                table: "patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "clinical",
                table: "patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncStatus",
                schema: "clinical",
                table: "patients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataCaptureSource",
                schema: "clinical",
                table: "patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_patients_OrganizationId_ClientOperationId",
                schema: "clinical",
                table: "patients",
                columns: new[] { "OrganizationId", "ClientOperationId" });

            migrationBuilder.CreateIndex(
                name: "IX_patients_OrganizationId_IdempotencyKey",
                schema: "clinical",
                table: "patients",
                columns: new[] { "OrganizationId", "IdempotencyKey" });

            migrationBuilder.CreateIndex(
                name: "IX_patients_OrganizationId_LocalPatientId",
                schema: "clinical",
                table: "patients",
                columns: new[] { "OrganizationId", "LocalPatientId" });

            migrationBuilder.CreateIndex(
                name: "IX_patients_OrganizationId_SourceBrigadeId",
                schema: "clinical",
                table: "patients",
                columns: new[] { "OrganizationId", "SourceBrigadeId" });

            migrationBuilder.CreateIndex(
                name: "IX_patients_OrganizationId_SyncStatus",
                schema: "clinical",
                table: "patients",
                columns: new[] { "OrganizationId", "SyncStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_patients_OrganizationId_ClientOperationId", schema: "clinical", table: "patients");
            migrationBuilder.DropIndex(name: "IX_patients_OrganizationId_IdempotencyKey", schema: "clinical", table: "patients");
            migrationBuilder.DropIndex(name: "IX_patients_OrganizationId_LocalPatientId", schema: "clinical", table: "patients");
            migrationBuilder.DropIndex(name: "IX_patients_OrganizationId_SourceBrigadeId", schema: "clinical", table: "patients");
            migrationBuilder.DropIndex(name: "IX_patients_OrganizationId_SyncStatus", schema: "clinical", table: "patients");

            migrationBuilder.DropColumn(name: "ClientOperationId", schema: "clinical", table: "patients");
            migrationBuilder.DropColumn(name: "DataCaptureSource", schema: "clinical", table: "patients");
            migrationBuilder.DropColumn(name: "IdempotencyKey", schema: "clinical", table: "patients");
            migrationBuilder.DropColumn(name: "LocalPatientId", schema: "clinical", table: "patients");
            migrationBuilder.DropColumn(name: "SourceBrigadeId", schema: "clinical", table: "patients");
            migrationBuilder.DropColumn(name: "SyncStatus", schema: "clinical", table: "patients");
        }
    }
}