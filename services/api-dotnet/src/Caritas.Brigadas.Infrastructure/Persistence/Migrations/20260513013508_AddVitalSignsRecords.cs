using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVitalSignsRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vital_signs",
                schema: "clinical",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncounterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MeasuredByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MeasuredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SystolicBloodPressureMmHg = table.Column<int>(type: "int", nullable: true),
                    DiastolicBloodPressureMmHg = table.Column<int>(type: "int", nullable: true),
                    HeartRateBpm = table.Column<int>(type: "int", nullable: true),
                    RespiratoryRatePerMinute = table.Column<int>(type: "int", nullable: true),
                    TemperatureCelsius = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    OxygenSaturationPercent = table.Column<int>(type: "int", nullable: true),
                    WeightKg = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    HeightCm = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    GlucoseMgDl = table.Column<decimal>(type: "decimal(7,2)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedOffline = table.Column<bool>(type: "bit", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SyncStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vital_signs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vital_signs_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "core",
                        principalTable: "organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vital_signs_patient_visits_VisitId",
                        column: x => x.VisitId,
                        principalSchema: "clinical",
                        principalTable: "patient_visits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vital_signs_patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vital_signs_service_encounters_EncounterId",
                        column: x => x.EncounterId,
                        principalSchema: "clinical",
                        principalTable: "service_encounters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_vital_signs_users_MeasuredByUserId",
                        column: x => x.MeasuredByUserId,
                        principalSchema: "core",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_EncounterId",
                schema: "clinical",
                table: "vital_signs",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_IsDeleted",
                schema: "clinical",
                table: "vital_signs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_MeasuredByUserId",
                schema: "clinical",
                table: "vital_signs",
                column: "MeasuredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_OrganizationId_PatientId_MeasuredAt",
                schema: "clinical",
                table: "vital_signs",
                columns: new[] { "OrganizationId", "PatientId", "MeasuredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_OrganizationId_SyncStatus",
                schema: "clinical",
                table: "vital_signs",
                columns: new[] { "OrganizationId", "SyncStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_PatientId",
                schema: "clinical",
                table: "vital_signs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_vital_signs_VisitId",
                schema: "clinical",
                table: "vital_signs",
                column: "VisitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vital_signs",
                schema: "clinical");
        }
    }
}
