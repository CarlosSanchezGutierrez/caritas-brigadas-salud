using Caritas.Brigadas.Api.Extensions;
using Caritas.Brigadas.Application.ConsentDocuments;
using Caritas.Brigadas.Contracts.Api;
using Caritas.Brigadas.Contracts.ConsentDocuments;
using Caritas.Brigadas.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Caritas.Brigadas.Api.Controllers;

/// <summary>
/// Endpoints para documentos de consentimiento y aviso de privacidad.
/// </summary>
[ApiController]
[Produces("application/json")]
public sealed class ConsentDocumentsController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ConsentDocumentsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lista consentimientos de una organización.
    /// </summary>
    [HttpGet("api/v1/organizations/{organizationId:guid}/consent-documents")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ConsentDocumentSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IConsentDocumentReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var documents = await repository.ListByOrganizationAsync(
            organizationId,
            cancellationToken);

        return Ok(ApiResponse<IReadOnlyCollection<ConsentDocumentSummaryDto>>.Ok(
            documents,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Obtiene un consentimiento por identificador.
    /// </summary>
    [HttpGet("api/v1/consent-documents/{consentDocumentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ConsentDocumentSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(
        Guid consentDocumentId,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IConsentDocumentReadRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        var document = await repository.GetByIdAsync(
            consentDocumentId,
            cancellationToken);

        if (document is null)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                "Consent document was not found.",
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }

        return Ok(ApiResponse<ConsentDocumentSummaryDto>.Ok(
            document,
            HttpContext.GetCorrelationId()));
    }

    /// <summary>
    /// Crea un documento de consentimiento o aviso de privacidad firmado.
    /// </summary>
    [HttpPost("api/v1/organizations/{organizationId:guid}/consent-documents")]
    [ProducesResponseType(typeof(ApiResponse<ConsentDocumentSummaryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateAsync(
        Guid organizationId,
        [FromBody] CreateConsentDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetService<IConsentDocumentWriteRepository>();

        if (repository is null)
        {
            return DatabaseNotConfigured();
        }

        try
        {
            var document = await repository.CreateAsync(
                organizationId,
                request,
                cancellationToken);

            var payload = ApiResponse<ConsentDocumentSummaryDto>.Ok(
                document,
                HttpContext.GetCorrelationId(),
                "Consent document created successfully.");

            return Created($"/api/v1/consent-documents/{document.Id}", payload);
        }
        catch (KeyNotFoundException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.NotFound,
                exception.Message,
                HttpContext.GetCorrelationId());

            return NotFound(error);
        }
        catch (InvalidOperationException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.Conflict,
                exception.Message,
                HttpContext.GetCorrelationId());

            return Conflict(error);
        }
        catch (DomainException exception)
        {
            var error = ApiErrorResponse.Create(
                ApiErrorCodes.ValidationError,
                exception.Message,
                HttpContext.GetCorrelationId());

            return BadRequest(error);
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
