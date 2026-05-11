using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations;

[DbContext(typeof(CaritasDbContext))]
[Migration("20260508054350_MakeSyncBatchDeviceIdNullable")]
public partial class MakeSyncBatchDeviceIdNullable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "DeviceId",
            schema: "sync",
            table: "sync_batches",
            type: "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE [sync].[sync_batches] SET [DeviceId] = '00000000-0000-0000-0000-000000000000' WHERE [DeviceId] IS NULL");

        migrationBuilder.AlterColumn<Guid>(
            name: "DeviceId",
            schema: "sync",
            table: "sync_batches",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);
    }
}