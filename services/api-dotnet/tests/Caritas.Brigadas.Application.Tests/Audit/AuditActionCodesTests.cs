using Caritas.Brigadas.Application.Audit;
using Xunit;

namespace Caritas.Brigadas.Application.Tests.Audit;

public sealed class AuditActionCodesTests
{
    [Fact]
    public void All_IncludesReportAuditActions()
    {
        Assert.Contains(AuditActionCodes.ReportSummaryRead, AuditActionCodes.All);
        Assert.Contains(AuditActionCodes.ReportSummaryExport, AuditActionCodes.All);
    }

    [Fact]
    public void ReportAuditActions_HaveExpectedValues()
    {
        Assert.Equal("reports.summary.read", AuditActionCodes.ReportSummaryRead);
        Assert.Equal("reports.summary.export", AuditActionCodes.ReportSummaryExport);
    }
}
