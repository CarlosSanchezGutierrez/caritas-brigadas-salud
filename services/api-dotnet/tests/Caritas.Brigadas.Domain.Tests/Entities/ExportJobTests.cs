using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class ExportJobTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePendingExportJob()
    {
        var organizationId = Guid.NewGuid();
        var requestedByUserId = Guid.NewGuid();
        var requestedAt = DateTimeOffset.UtcNow;

        var exportJob = new ExportJob(
            Guid.NewGuid(),
            organizationId,
            requestedByUserId,
            " Daily_Report ",
            requestedAt,
            filtersJson: """{ "brigadeId": "123" }""",
            includesIdentifiableData: true);

        Assert.Equal(organizationId, exportJob.OrganizationId);
        Assert.Equal(requestedByUserId, exportJob.RequestedByUserId);
        Assert.Equal(ExportType.DailyReport, exportJob.ExportType);
        Assert.Equal("""{ "brigadeId": "123" }""", exportJob.FiltersJson);
        Assert.True(exportJob.IncludesIdentifiableData);
        Assert.Equal(ExportJobStatus.Pending, exportJob.Status);
        Assert.True(exportJob.IsPending);
    }

    [Fact]
    public void Constructor_WithEmptyRequestedByUserId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new ExportJob(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                ExportType.DailyReport,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void MarkProcessing_WhenPending_ShouldSetProcessing()
    {
        var exportJob = CreateExportJob();

        exportJob.MarkProcessing();

        Assert.Equal(ExportJobStatus.Processing, exportJob.Status);
    }

    [Fact]
    public void Complete_ShouldSetCompletedMetadata()
    {
        var exportJob = CreateExportJob();
        var completedAt = DateTimeOffset.UtcNow;

        exportJob.MarkProcessing();
        exportJob.Complete("exports/report.xlsx", completedAt);

        Assert.Equal(ExportJobStatus.Completed, exportJob.Status);
        Assert.Equal("exports/report.xlsx", exportJob.FileUrl);
        Assert.Equal(completedAt, exportJob.CompletedAt);
        Assert.True(exportJob.IsCompleted);
    }

    [Fact]
    public void Fail_ShouldSetFailedMetadata()
    {
        var exportJob = CreateExportJob();
        var completedAt = DateTimeOffset.UtcNow;

        exportJob.Fail("Storage unavailable", completedAt);

        Assert.Equal(ExportJobStatus.Failed, exportJob.Status);
        Assert.Equal("Storage unavailable", exportJob.ErrorMessage);
        Assert.Equal(completedAt, exportJob.CompletedAt);
        Assert.True(exportJob.IsFailed);
    }

    [Fact]
    public void Cancel_AfterCompleted_ShouldThrowDomainException()
    {
        var exportJob = CreateExportJob();

        exportJob.Complete("exports/report.xlsx", DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(exportJob.Cancel);
    }

    private static ExportJob CreateExportJob()
    {
        return new ExportJob(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            ExportType.DailyReport,
            DateTimeOffset.UtcNow);
    }
}
