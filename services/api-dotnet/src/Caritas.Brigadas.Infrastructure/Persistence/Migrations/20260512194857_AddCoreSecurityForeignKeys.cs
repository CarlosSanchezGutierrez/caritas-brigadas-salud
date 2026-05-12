using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreSecurityForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                schema: "core",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserId",
                schema: "core",
                table: "user_roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                schema: "core",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                schema: "core",
                table: "role_permissions",
                column: "PermissionId",
                principalSchema: "core",
                principalTable: "permissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                schema: "core",
                table: "role_permissions",
                column: "RoleId",
                principalSchema: "core",
                principalTable: "roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_organizations_OrganizationId",
                schema: "core",
                table: "roles",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_services_organizations_OrganizationId",
                schema: "core",
                table: "services",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_organizations_OrganizationId",
                schema: "core",
                table: "user_roles",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_RoleId",
                schema: "core",
                table: "user_roles",
                column: "RoleId",
                principalSchema: "core",
                principalTable: "roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_UserId",
                schema: "core",
                table: "user_roles",
                column: "UserId",
                principalSchema: "core",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_organizations_OrganizationId",
                schema: "core",
                table: "users",
                column: "OrganizationId",
                principalSchema: "core",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_PermissionId",
                schema: "core",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_RoleId",
                schema: "core",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_organizations_OrganizationId",
                schema: "core",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_services_organizations_OrganizationId",
                schema: "core",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_organizations_OrganizationId",
                schema: "core",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_RoleId",
                schema: "core",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_UserId",
                schema: "core",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_organizations_OrganizationId",
                schema: "core",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_user_roles_RoleId",
                schema: "core",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "IX_user_roles_UserId",
                schema: "core",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "IX_role_permissions_PermissionId",
                schema: "core",
                table: "role_permissions");
        }
    }
}
