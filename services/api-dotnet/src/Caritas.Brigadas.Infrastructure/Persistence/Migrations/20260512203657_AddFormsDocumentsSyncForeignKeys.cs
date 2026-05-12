using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFormsDocumentsSyncForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sync_events_OrganizationId",
                schema: "sync",
                table: "sync_events",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_sync_batches_BrigadeId",
                schema: "sync",
                table: "sync_batches",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_media_releases_PatientId",
                schema: "documents",
                table: "media_releases",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_media_releases_VisitId",
                schema: "documents",
                table: "media_releases",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_form_templates_ServiceId",
                schema: "forms",
                table: "form_templates",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_form_responses_EncounterId",
                schema: "forms",
                table: "form_responses",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_form_responses_FormTemplateId",
                schema: "forms",
                table: "form_responses",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_document_templates_AppliesToServiceId",
                schema: "documents",
                table: "document_templates",
                column: "AppliesToServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_document_signatures_DocumentTemplateId",
                schema: "documents",
                table: "document_signatures",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_document_signatures_EncounterId",
                schema: "documents",
                table: "document_signatures",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_document_signatures_VisitId",
                schema: "documents",
                table: "document_signatures",
                column: "VisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_document_signatures_document_templates_DocumentTemplateId",
                schema: "documents",
                table: "document_signatures",
                column: "DocumentTemplateId",
                principalSchema: "documents",
                principalTable: "document_templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_signatures_organizations_OrganizationId",
                schema: "documents",
                table: "document_signatures",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_signatures_patient_visits_VisitId",
                schema: "documents",
                table: "document_signatures",
                column: "VisitId",
                principalSchema: "clinical",
                principalTable: "patient_visits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_signatures_patients_PatientId",
                schema: "documents",
                table: "document_signatures",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_signatures_service_encounters_EncounterId",
                schema: "documents",
                table: "document_signatures",
                column: "EncounterId",
                principalSchema: "clinical",
                principalTable: "service_encounters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_templates_organizations_OrganizationId",
                schema: "documents",
                table: "document_templates",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_document_templates_services_AppliesToServiceId",
                schema: "documents",
                table: "document_templates",
                column: "AppliesToServiceId",
                principalSchema: "core",
                principalTable: "services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_responses_form_templates_FormTemplateId",
                schema: "forms",
                table: "form_responses",
                column: "FormTemplateId",
                principalSchema: "forms",
                principalTable: "form_templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_responses_organizations_OrganizationId",
                schema: "forms",
                table: "form_responses",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_responses_service_encounters_EncounterId",
                schema: "forms",
                table: "form_responses",
                column: "EncounterId",
                principalSchema: "clinical",
                principalTable: "service_encounters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_templates_organizations_OrganizationId",
                schema: "forms",
                table: "form_templates",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_form_templates_services_ServiceId",
                schema: "forms",
                table: "form_templates",
                column: "ServiceId",
                principalSchema: "core",
                principalTable: "services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_media_releases_organizations_OrganizationId",
                schema: "documents",
                table: "media_releases",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_media_releases_patient_visits_VisitId",
                schema: "documents",
                table: "media_releases",
                column: "VisitId",
                principalSchema: "clinical",
                principalTable: "patient_visits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_media_releases_patients_PatientId",
                schema: "documents",
                table: "media_releases",
                column: "PatientId",
                principalSchema: "clinical",
                principalTable: "patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sync_batches_brigades_BrigadeId",
                schema: "sync",
                table: "sync_batches",
                column: "BrigadeId",
                principalSchema: "brigades",
                principalTable: "brigades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sync_batches_organizations_OrganizationId",
                schema: "sync",
                table: "sync_batches",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sync_events_organizations_OrganizationId",
                schema: "sync",
                table: "sync_events",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sync_events_sync_batches_SyncBatchId",
                schema: "sync",
                table: "sync_events",
                column: "SyncBatchId",
                principalSchema: "sync",
                principalTable: "sync_batches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_signatures_document_templates_DocumentTemplateId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropForeignKey(
                name: "FK_document_signatures_organizations_OrganizationId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropForeignKey(
                name: "FK_document_signatures_patient_visits_VisitId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropForeignKey(
                name: "FK_document_signatures_patients_PatientId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropForeignKey(
                name: "FK_document_signatures_service_encounters_EncounterId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropForeignKey(
                name: "FK_document_templates_organizations_OrganizationId",
                schema: "documents",
                table: "document_templates");

            migrationBuilder.DropForeignKey(
                name: "FK_document_templates_services_AppliesToServiceId",
                schema: "documents",
                table: "document_templates");

            migrationBuilder.DropForeignKey(
                name: "FK_form_responses_form_templates_FormTemplateId",
                schema: "forms",
                table: "form_responses");

            migrationBuilder.DropForeignKey(
                name: "FK_form_responses_organizations_OrganizationId",
                schema: "forms",
                table: "form_responses");

            migrationBuilder.DropForeignKey(
                name: "FK_form_responses_service_encounters_EncounterId",
                schema: "forms",
                table: "form_responses");

            migrationBuilder.DropForeignKey(
                name: "FK_form_templates_organizations_OrganizationId",
                schema: "forms",
                table: "form_templates");

            migrationBuilder.DropForeignKey(
                name: "FK_form_templates_services_ServiceId",
                schema: "forms",
                table: "form_templates");

            migrationBuilder.DropForeignKey(
                name: "FK_media_releases_organizations_OrganizationId",
                schema: "documents",
                table: "media_releases");

            migrationBuilder.DropForeignKey(
                name: "FK_media_releases_patient_visits_VisitId",
                schema: "documents",
                table: "media_releases");

            migrationBuilder.DropForeignKey(
                name: "FK_media_releases_patients_PatientId",
                schema: "documents",
                table: "media_releases");

            migrationBuilder.DropForeignKey(
                name: "FK_sync_batches_brigades_BrigadeId",
                schema: "sync",
                table: "sync_batches");

            migrationBuilder.DropForeignKey(
                name: "FK_sync_batches_organizations_OrganizationId",
                schema: "sync",
                table: "sync_batches");

            migrationBuilder.DropForeignKey(
                name: "FK_sync_events_organizations_OrganizationId",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.DropForeignKey(
                name: "FK_sync_events_sync_batches_SyncBatchId",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.DropIndex(
                name: "IX_sync_events_OrganizationId",
                schema: "sync",
                table: "sync_events");

            migrationBuilder.DropIndex(
                name: "IX_sync_batches_BrigadeId",
                schema: "sync",
                table: "sync_batches");

            migrationBuilder.DropIndex(
                name: "IX_media_releases_PatientId",
                schema: "documents",
                table: "media_releases");

            migrationBuilder.DropIndex(
                name: "IX_media_releases_VisitId",
                schema: "documents",
                table: "media_releases");

            migrationBuilder.DropIndex(
                name: "IX_form_templates_ServiceId",
                schema: "forms",
                table: "form_templates");

            migrationBuilder.DropIndex(
                name: "IX_form_responses_EncounterId",
                schema: "forms",
                table: "form_responses");

            migrationBuilder.DropIndex(
                name: "IX_form_responses_FormTemplateId",
                schema: "forms",
                table: "form_responses");

            migrationBuilder.DropIndex(
                name: "IX_document_templates_AppliesToServiceId",
                schema: "documents",
                table: "document_templates");

            migrationBuilder.DropIndex(
                name: "IX_document_signatures_DocumentTemplateId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropIndex(
                name: "IX_document_signatures_EncounterId",
                schema: "documents",
                table: "document_signatures");

            migrationBuilder.DropIndex(
                name: "IX_document_signatures_VisitId",
                schema: "documents",
                table: "document_signatures");
        }
    }
}
