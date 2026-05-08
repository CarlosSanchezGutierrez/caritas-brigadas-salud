using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caritas.Brigadas.Infrastructure.Persistence.Migrations;

[Migration("20260508062505_AddFormResponseSubmittedTimestamps")]
public partial class AddFormResponseSubmittedTimestamps : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "SubmittedAt",
            schema: "forms",
            table: "form_responses",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "CapturedAt",
            schema: "forms",
            table: "form_responses",
            type: "datetimeoffset",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SubmittedAt",
            schema: "forms",
            table: "form_responses");

        migrationBuilder.DropColumn(
            name: "CapturedAt",
            schema: "forms",
            table: "form_responses");
    }
}