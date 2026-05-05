using System.Text;
using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.Reports;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para reportes operativos agregados.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:guid}/reports")]
[Produces("application/json")]
public sealed class ReportsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ReportsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Obtiene resumen operativo general de una organización.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<OrganizationReportSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetSummaryAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IReportReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var summary = await repository.GetOrganizationSummaryAsync(
                organizationId,
                cancellationToken);

            return Ok(ApiResponse<OrganizationReportSummaryDto>.Ok(
                summary,
                HttpContext.GetCorrelationId()));
        }
        catch (KeyNotFoundException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                exception.Message,
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }
    }

    /// <summary>
    /// Exporta el resumen operativo general de una organización en CSV.
    /// </summary>
    [HttpGet("summary.csv")]
    [Produces("text/csv")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ExportSummaryCsvAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IReportReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var summary = await repository.GetOrganizationSummaryAsync(
                organizationId,
                cancellationToken);

            var csv = BuildSummaryCsv(summary);
            var bytes = Encoding.UTF8.GetPreamble()
                .Concat(Encoding.UTF8.GetBytes(csv))
                .ToArray();

            var fileName = $"caritas-report-summary-{organizationId}-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.csv";

            return File(
                bytes,
                "text/csv; charset=utf-8",
                fileName);
        }
        catch (KeyNotFoundException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                exception.Message,
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }
    }

    private static string BuildSummaryCsv(OrganizationReportSummaryDto summary)
    {
        var builder = new StringBuilder();

        builder.AppendLine("metric,value");
        AppendMetric(builder, "organizationId", summary.OrganizationId.ToString());
        AppendMetric(builder, "generatedAtUtc", summary.GeneratedAtUtc.ToString("O"));
        AppendMetric(builder, "usersCount", summary.UsersCount.ToString());
        AppendMetric(builder, "rolesCount", summary.RolesCount.ToString());
        AppendMetric(builder, "permissionsCount", summary.PermissionsCount.ToString());
        AppendMetric(builder, "rolePermissionsCount", summary.RolePermissionsCount.ToString());
        AppendMetric(builder, "servicesCount", summary.ServicesCount.ToString());
        AppendMetric(builder, "communitiesCount", summary.CommunitiesCount.ToString());
        AppendMetric(builder, "mobileUnitsCount", summary.MobileUnitsCount.ToString());
        AppendMetric(builder, "brigadesCount", summary.BrigadesCount.ToString());
        AppendMetric(builder, "brigadeServiceAssignmentsCount", summary.BrigadeServiceAssignmentsCount.ToString());
        AppendMetric(builder, "patientsCount", summary.PatientsCount.ToString());
        AppendMetric(builder, "patientVisitsCount", summary.PatientVisitsCount.ToString());
        AppendMetric(builder, "serviceEncountersCount", summary.ServiceEncountersCount.ToString());
        AppendMetric(builder, "formTemplatesCount", summary.FormTemplatesCount.ToString());
        AppendMetric(builder, "formResponsesCount", summary.FormResponsesCount.ToString());
        AppendMetric(builder, "consentDocumentsCount", summary.ConsentDocumentsCount.ToString());
        AppendMetric(builder, "clinicalRecordsCount", summary.ClinicalRecordsCount.ToString());

        return builder.ToString();
    }

    private static void AppendMetric(
        StringBuilder builder,
        string metric,
        string value)
    {
        builder
            .Append(EscapeCsv(metric))
            .Append(',')
            .Append(EscapeCsv(value))
            .AppendLine();
    }

    private static string EscapeCsv(string value)
    {
        if (!value.Contains(',') &&
            !value.Contains('"') &&
            !value.Contains('\r') &&
            !value.Contains('\n'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private ObjectResult DatabaseNotConfigured()
    {
        var error = ApiErrorResponse.Create(
            "database_not_configured",
            "Database access is not configured for this environment.",
            HttpContext.GetCorrelationId());

        return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
    }
}
