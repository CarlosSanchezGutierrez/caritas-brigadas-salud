using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBrigadesForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_brigades_CommunityId",
                schema: "brigades",
                table: "brigades",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_brigades_MobileUnitId",
                schema: "brigades",
                table: "brigades",
                column: "MobileUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_brigade_services_ServiceId",
                schema: "brigades",
                table: "brigade_services",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_brigade_services_brigades_BrigadeId",
                schema: "brigades",
                table: "brigade_services",
                column: "BrigadeId",
                principalSchema: "brigades",
                principalTable: "brigades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_brigade_services_services_ServiceId",
                schema: "brigades",
                table: "brigade_services",
                column: "ServiceId",
                principalSchema: "core",
                principalTable: "services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_brigades_communities_CommunityId",
                schema: "brigades",
                table: "brigades",
                column: "CommunityId",
                principalSchema: "brigades",
                principalTable: "communities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_brigades_mobile_units_MobileUnitId",
                schema: "brigades",
                table: "brigades",
                column: "MobileUnitId",
                principalSchema: "brigades",
                principalTable: "mobile_units",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_brigades_organizations_OrganizationId",
                schema: "brigades",
                table: "brigades",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_communities_organizations_OrganizationId",
                schema: "brigades",
                table: "communities",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mobile_units_organizations_OrganizationId",
                schema: "brigades",
                table: "mobile_units",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_brigade_services_brigades_BrigadeId",
                schema: "brigades",
                table: "brigade_services");

            migrationBuilder.DropForeignKey(
                name: "FK_brigade_services_services_ServiceId",
                schema: "brigades",
                table: "brigade_services");

            migrationBuilder.DropForeignKey(
                name: "FK_brigades_communities_CommunityId",
                schema: "brigades",
                table: "brigades");

            migrationBuilder.DropForeignKey(
                name: "FK_brigades_mobile_units_MobileUnitId",
                schema: "brigades",
                table: "brigades");

            migrationBuilder.DropForeignKey(
                name: "FK_brigades_organizations_OrganizationId",
                schema: "brigades",
                table: "brigades");

            migrationBuilder.DropForeignKey(
                name: "FK_communities_organizations_OrganizationId",
                schema: "brigades",
                table: "communities");

            migrationBuilder.DropForeignKey(
                name: "FK_mobile_units_organizations_OrganizationId",
                schema: "brigades",
                table: "mobile_units");

            migrationBuilder.DropIndex(
                name: "IX_brigades_CommunityId",
                schema: "brigades",
                table: "brigades");

            migrationBuilder.DropIndex(
                name: "IX_brigades_MobileUnitId",
                schema: "brigades",
                table: "brigades");

            migrationBuilder.DropIndex(
                name: "IX_brigade_services_ServiceId",
                schema: "brigades",
                table: "brigade_services");
        }
    }
}
