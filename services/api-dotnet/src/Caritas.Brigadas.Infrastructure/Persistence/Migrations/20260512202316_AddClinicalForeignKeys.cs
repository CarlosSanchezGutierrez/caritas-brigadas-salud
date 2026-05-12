using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_service_encounters_BrigadeId",
                schema: "clinical",
                table: "service_encounters",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_service_encounters_PatientId",
                schema: "clinical",
                table: "service_encounters",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_service_encounters_ServiceId",
                schema: "clinical",
                table: "service_encounters",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_patient_visits_PatientId",
                schema: "clinical",
                table: "patient_visits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_medication_deliveries_EncounterId",
                schema: "clinical",
                table: "medication_deliveries",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_medication_deliveries_PatientId",
                schema: "clinical",
                table: "medication_deliveries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_medical_referrals_EncounterId",
                schema: "clinical",
                table: "medical_referrals",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_medical_referrals_PatientId",
                schema: "clinical",
                table: "medical_referrals",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_medical_referrals_organizations_OrganizationId",
                schema: "clinical",
                table: "medical_referrals",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_medical_referrals_patients_PatientId",
                schema: "clinical",
                table: "medical_referrals",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_medical_referrals_service_encounters_EncounterId",
                schema: "clinical",
                table: "medical_referrals",
                column: "EncounterId",
                principalSchema: "clinical",
                principalTable: "service_encounters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_medication_deliveries_organizations_OrganizationId",
                schema: "clinical",
                table: "medication_deliveries",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_medication_deliveries_patients_PatientId",
                schema: "clinical",
                table: "medication_deliveries",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_medication_deliveries_service_encounters_EncounterId",
                schema: "clinical",
                table: "medication_deliveries",
                column: "EncounterId",
                principalSchema: "clinical",
                principalTable: "service_encounters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_guardians_patients_PatientId",
                schema: "clinical",
                table: "patient_guardians",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_visits_brigades_BrigadeId",
                schema: "clinical",
                table: "patient_visits",
                column: "BrigadeId",
                principalSchema: "brigades",
                principalTable: "brigades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_visits_organizations_OrganizationId",
                schema: "clinical",
                table: "patient_visits",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patient_visits_patients_PatientId",
                schema: "clinical",
                table: "patient_visits",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_organizations_OrganizationId",
                schema: "clinical",
                table: "patients",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_encounters_brigades_BrigadeId",
                schema: "clinical",
                table: "service_encounters",
                column: "BrigadeId",
                principalSchema: "brigades",
                principalTable: "brigades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_encounters_organizations_OrganizationId",
                schema: "clinical",
                table: "service_encounters",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_encounters_patient_visits_VisitId",
                schema: "clinical",
                table: "service_encounters",
                column: "VisitId",
                principalSchema: "clinical",
                principalTable: "patient_visits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_encounters_patients_PatientId",
                schema: "clinical",
                table: "service_encounters",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_encounters_services_ServiceId",
                schema: "clinical",
                table: "service_encounters",
                column: "ServiceId",
                principalSchema: "core",
                principalTable: "services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medical_referrals_organizations_OrganizationId",
                schema: "clinical",
                table: "medical_referrals");

            migrationBuilder.DropForeignKey(
                name: "FK_medical_referrals_patients_PatientId",
                schema: "clinical",
                table: "medical_referrals");

            migrationBuilder.DropForeignKey(
                name: "FK_medical_referrals_service_encounters_EncounterId",
                schema: "clinical",
                table: "medical_referrals");

            migrationBuilder.DropForeignKey(
                name: "FK_medication_deliveries_organizations_OrganizationId",
                schema: "clinical",
                table: "medication_deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_medication_deliveries_patients_PatientId",
                schema: "clinical",
                table: "medication_deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_medication_deliveries_service_encounters_EncounterId",
                schema: "clinical",
                table: "medication_deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_guardians_patients_PatientId",
                schema: "clinical",
                table: "patient_guardians");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_visits_brigades_BrigadeId",
                schema: "clinical",
                table: "patient_visits");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_visits_organizations_OrganizationId",
                schema: "clinical",
                table: "patient_visits");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_visits_patients_PatientId",
                schema: "clinical",
                table: "patient_visits");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_organizations_OrganizationId",
                schema: "clinical",
                table: "patients");

            migrationBuilder.DropForeignKey(
                name: "FK_service_encounters_brigades_BrigadeId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropForeignKey(
                name: "FK_service_encounters_organizations_OrganizationId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropForeignKey(
                name: "FK_service_encounters_patient_visits_VisitId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropForeignKey(
                name: "FK_service_encounters_patients_PatientId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropForeignKey(
                name: "FK_service_encounters_services_ServiceId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropIndex(
                name: "IX_service_encounters_BrigadeId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropIndex(
                name: "IX_service_encounters_PatientId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropIndex(
                name: "IX_service_encounters_ServiceId",
                schema: "clinical",
                table: "service_encounters");

            migrationBuilder.DropIndex(
                name: "IX_patient_visits_PatientId",
                schema: "clinical",
                table: "patient_visits");

            migrationBuilder.DropIndex(
                name: "IX_medication_deliveries_EncounterId",
                schema: "clinical",
                table: "medication_deliveries");

            migrationBuilder.DropIndex(
                name: "IX_medication_deliveries_PatientId",
                schema: "clinical",
                table: "medication_deliveries");

            migrationBuilder.DropIndex(
                name: "IX_medical_referrals_EncounterId",
                schema: "clinical",
                table: "medical_referrals");

            migrationBuilder.DropIndex(
                name: "IX_medical_referrals_PatientId",
                schema: "clinical",
                table: "medical_referrals");
        }
    }
}
