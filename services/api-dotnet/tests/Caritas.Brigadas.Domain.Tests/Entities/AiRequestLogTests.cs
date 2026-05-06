using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class AiRequestLogTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRequestedLog()
    {
        var organizationId = Guid.NewGuid();
        var requestedByUserId = Guid.NewGuid();
        var requestedAt = DateTimeOffset.UtcNow;

        var log = new AiRequestLog(
            Guid.NewGuid(),
            organizationId,
            requestedByUserId,
            " Report_Summary ",
            " Resumir reporte operativo ",
            requestedAt,
            provider: "OpenAI",
            model: "gpt-model",
            promptHash: "PROMPT_HASH",
            inputHash: "INPUT_HASH",
            containsSensitiveData: false);

        Assert.Equal(organizationId, log.OrganizationId);
        Assert.Equal(requestedByUserId, log.RequestedByUserId);
        Assert.Equal("report_summary", log.Module);
        Assert.Equal("Resumir reporte operativo", log.Purpose);
        Assert.Equal("openai", log.Provider);
        Assert.Equal("gpt-model", log.Model);
        Assert.Equal("PROMPT_HASH", log.PromptHash);
        Assert.Equal("INPUT_HASH", log.InputHash);
        Assert.False(log.ContainsSensitiveData);
        Assert.Equal(AiRequestStatus.Requested, log.Status);
        Assert.Equal(requestedAt, log.RequestedAt);
    }

    [Fact]
    public void Constructor_WithEmptyRequestedByUserId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AiRequestLog(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                AiModule.ReportSummary,
                "Resumen",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkCompleted_ShouldSetCompletedMetadata()
    {
        var log = CreateLog();
        var completedAt = DateTimeOffset.UtcNow;

        log.MarkCompleted(completedAt, "OUTPUT_HASH");

        Assert.Equal(AiRequestStatus.Completed, log.Status);
        Assert.Equal(completedAt, log.CompletedAt);
        Assert.Equal("OUTPUT_HASH", log.OutputHash);
        Assert.True(log.IsCompleted);
    }

    [Fact]
    public void MarkFailed_ShouldSetFailureMetadata()
    {
        var log = CreateLog();
        var completedAt = DateTimeOffset.UtcNow;

        log.MarkFailed(completedAt, "Provider timeout");

        Assert.Equal(AiRequestStatus.Failed, log.Status);
        Assert.Equal(completedAt, log.CompletedAt);
        Assert.Equal("Provider timeout", log.ErrorMessage);
        Assert.True(log.IsFailed);
    }

    [Fact]
    public void MarkBlocked_ShouldSetBlockedStatus()
    {
        var log = CreateLog();
        var completedAt = DateTimeOffset.UtcNow;

        log.MarkBlocked(completedAt, "Sensitive clinical decision blocked");

        Assert.Equal(AiRequestStatus.Blocked, log.Status);
        Assert.Equal("Sensitive clinical decision blocked", log.ErrorMessage);
        Assert.True(log.IsBlocked);
    }

    [Fact]
    public void MarkCompleted_AfterBlocked_ShouldThrowDomainException()
    {
        var log = CreateLog();

        log.MarkBlocked(DateTimeOffset.UtcNow, "Blocked");

        Assert.Throws<DomainException>(() =>
            log.MarkCompleted(DateTimeOffset.UtcNow));
    }

    private static AiRequestLog CreateLog()
    {
        return new AiRequestLog(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            AiModule.ReportSummary,
            "Resumen operativo",
            DateTimeOffset.UtcNow);
    }
}
