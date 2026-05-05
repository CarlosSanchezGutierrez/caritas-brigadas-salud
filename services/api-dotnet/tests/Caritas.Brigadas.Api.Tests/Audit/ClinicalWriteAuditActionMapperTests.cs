using Caritas.Brigadas.Api.Audit;
using Caritas.Brigadas.Application.Audit;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Audit;

public sealed class ClinicalWriteAuditActionMapperTests
{
    [Theory]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/patients", AuditActionCodes.PatientCreate, "Patient")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/patient-visits", AuditActionCodes.PatientVisitCreate, "PatientVisit")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/service-encounters", AuditActionCodes.ServiceEncounterCreate, "ServiceEncounter")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/form-responses", AuditActionCodes.FormResponseCreate, "FormResponse")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/consent-documents", AuditActionCodes.ConsentDocumentCreate, "ConsentDocument")]
    public void TryMap_WhenClinicalWriteEndpoint_ReturnsExpectedAction(
        string method,
        string path,
        string expectedAction,
        string expectedEntityName)
    {
        var result = ClinicalWriteAuditActionMapper.TryMap(
            method,
            path,
            out var action,
            out var entityName);

        Assert.True(result);
        Assert.Equal(expectedAction, action);
        Assert.Equal(expectedEntityName, entityName);
    }

    [Theory]
    [InlineData("GET", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/patients")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/reports/summary")]
    [InlineData("POST", "")]
    public void TryMap_WhenNotClinicalWriteEndpoint_ReturnsFalse(
        string method,
        string path)
    {
        var result = ClinicalWriteAuditActionMapper.TryMap(
            method,
            path,
            out var action,
            out var entityName);

        Assert.False(result);
        Assert.Equal(string.Empty, action);
        Assert.Equal(string.Empty, entityName);
    }
}
