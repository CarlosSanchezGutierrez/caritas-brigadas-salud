using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations
{
    public partial class AddPatientCreateIdempotencyUniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_ClientOperationId'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    DROP INDEX [IX_patients_OrganizationId_ClientOperationId] ON [clinical].[patients];
                END;

                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_IdempotencyKey'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    DROP INDEX [IX_patients_OrganizationId_IdempotencyKey] ON [clinical].[patients];
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_ClientOperationId_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_patients_OrganizationId_ClientOperationId_UQ]
                    ON [clinical].[patients] ([OrganizationId], [ClientOperationId])
                    WHERE [ClientOperationId] IS NOT NULL AND [IsDeleted] = 0;
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_IdempotencyKey_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_patients_OrganizationId_IdempotencyKey_UQ]
                    ON [clinical].[patients] ([OrganizationId], [IdempotencyKey])
                    WHERE [IdempotencyKey] IS NOT NULL AND [IsDeleted] = 0;
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ]
                    ON [clinical].[patients] ([OrganizationId], [SourceBrigadeId], [LocalPatientId])
                    WHERE [SourceBrigadeId] IS NOT NULL AND [LocalPatientId] IS NOT NULL AND [IsDeleted] = 0;
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_ClientOperationId_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    DROP INDEX [IX_patients_OrganizationId_ClientOperationId_UQ] ON [clinical].[patients];
                END;

                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_IdempotencyKey_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    DROP INDEX [IX_patients_OrganizationId_IdempotencyKey_UQ] ON [clinical].[patients];
                END;

                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    DROP INDEX [IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ] ON [clinical].[patients];
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_ClientOperationId'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    CREATE INDEX [IX_patients_OrganizationId_ClientOperationId]
                    ON [clinical].[patients] ([OrganizationId], [ClientOperationId]);
                END;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = N'IX_patients_OrganizationId_IdempotencyKey'
                      AND object_id = OBJECT_ID(N'[clinical].[patients]')
                )
                BEGIN
                    CREATE INDEX [IX_patients_OrganizationId_IdempotencyKey]
                    ON [clinical].[patients] ([OrganizationId], [IdempotencyKey]);
                END;
                """);
        }
    }
}