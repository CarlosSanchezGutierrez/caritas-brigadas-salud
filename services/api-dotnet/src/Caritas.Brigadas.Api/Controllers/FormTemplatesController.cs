using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.FormTemplates;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.FormTemplates;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para plantillas JSON de formularios.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class FormTemplatesController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public FormTemplatesController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista plantillas de formularios de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/form-templates")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<FormTemplateSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormTemplateReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var templates = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<FormTemplateSummaryDto>>.Ok(
            templates,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene una plantilla de formulario por identificador.
    /// </summary>
    [HttpGet("api/v1/form-templates/{formTemplateId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FormTemplateSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid formTemplateId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormTemplateReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var template = await repository.GetByIdAsync(
            formTemplateId,
            cancellationToken);

        if (template is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Form template was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<FormTemplateSummaryDto>.Ok(
            template,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Inicializa plantillas base de formularios por servicio.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/form-templates/seed-defaults")]
    [ProducesResponseType(typeof(ApiResponse<FormTemplateSeedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> SeedDefaultsAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IFormTemplateSeedRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var result = await repository.SeedDefaultsAsync(
                organizationId,
                cancellationToken);

            return Ok(ApiResponse<FormTemplateSeedResultDto>.Ok(
                result,
                HttpContext.GetCorrelationId(),
                "Default form templates were seeded successfully."));
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

    private ObjectResult DatabaseNotConfigured()
    {
        var error = ApiErrorResponse.Create(
            "database_not_configured",
            "Database access is not configured for this environment.",
            HttpContext.GetCorrelationId());

        return StatusCode(StatusCodes.Status503ServiceUnavailable, error);
    }
}
