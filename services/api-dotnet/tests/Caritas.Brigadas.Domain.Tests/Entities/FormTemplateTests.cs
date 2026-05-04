using Caritas.Brigadas.Domain.Common;
using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class FormTemplateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateTemplate()
    {
        var template = new FormTemplate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            " consulta_general ",
            " Consulta general ",
            "1.0.0",
            """{ "type": "object" }""");

        Assert.Equal("CONSULTA_GENERAL", template.FormCode);
        Assert.Equal("Consulta general", template.Name);
        Assert.Equal("1.0.0", template.Version);
        Assert.True(template.IsActive);
    }

    [Fact]
    public void Constructor_WithInvalidEffectiveDates_ShouldThrowDomainException()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddDays(-1);

        Assert.Throws<DomainException>(() =>
            new FormTemplate(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "FORM",
                "Form",
                "1.0.0",
                "{}",
                effectiveFrom: start,
                effectiveTo: end));
    }

    [Fact]
    public void Deactivate_ShouldSetInactive()
    {
        var template = new FormTemplate(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "FORM",
            "Form",
            "1.0.0",
            "{}");

        template.Deactivate();

        Assert.False(template.IsActive);
    }
}
