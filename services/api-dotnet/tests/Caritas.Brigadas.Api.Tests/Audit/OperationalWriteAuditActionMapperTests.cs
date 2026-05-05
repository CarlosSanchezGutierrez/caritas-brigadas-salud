using Caritas.Brigadas.Api.Audit;
using Caritas.Brigadas.Application.Audit;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Audit;

public sealed class OperationalWriteAuditActionMapperTests
{
    [Theory]
    [InlineData("POST", "/api/v1/organizations", AuditActionCodes.OrganizationCreate, "Organization")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/users", AuditActionCodes.UserCreate, "User")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/security/seed-defaults", AuditActionCodes.RoleAssign, "SecurityDefaults")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/security/user-role-assignments", AuditActionCodes.RoleAssign, "UserRoleAssignment")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/services/seed-defaults", AuditActionCodes.ServiceSeed, "Service")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/form-templates/seed-defaults", AuditActionCodes.FormTemplateSeed, "FormTemplate")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/communities", AuditActionCodes.CommunityCreate, "Community")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/mobile-units", AuditActionCodes.MobileUnitCreate, "MobileUnit")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/brigades", AuditActionCodes.BrigadeCreate, "Brigade")]
    [InlineData("POST", "/api/v1/brigades/22222222-2222-2222-2222-222222222222/services", AuditActionCodes.BrigadeServiceAssign, "BrigadeService")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/sync-batches", AuditActionCodes.SyncBatchCreate, "SyncBatch")]
    public void TryMap_WhenOperationalWriteEndpoint_ReturnsExpectedAction(
        string method,
        string path,
        string expectedAction,
        string expectedEntityName)
    {
        var result = OperationalWriteAuditActionMapper.TryMap(
            method,
            path,
            out var action,
            out var entityName);

        Assert.True(result);
        Assert.Equal(expectedAction, action);
        Assert.Equal(expectedEntityName, entityName);
    }

    [Theory]
    [InlineData("GET", "/api/v1/organizations")]
    [InlineData("POST", "/api/v1/organizations/11111111-1111-1111-1111-111111111111/patients")]
    [InlineData("POST", "")]
    public void TryMap_WhenNotOperationalWriteEndpoint_ReturnsFalse(
        string method,
        string path)
    {
        var result = OperationalWriteAuditActionMapper.TryMap(
            method,
            path,
            out var action,
            out var entityName);

        Assert.False(result);
        Assert.Equal(string.Empty, action);
        Assert.Equal(string.Empty, entityName);
    }
}
