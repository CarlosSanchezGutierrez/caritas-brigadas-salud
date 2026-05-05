using Caritas.Brigadas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Caritas.Brigadas.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(auditLog => auditLog.Id);

        builder.Property(auditLog => auditLog.OrganizationId)
            .IsRequired();

        builder.Property(auditLog => auditLog.UserId);

        builder.Property(auditLog => auditLog.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(auditLog => auditLog.EntityName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(auditLog => auditLog.EntityId);

        builder.Property(auditLog => auditLog.DetailsJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(auditLog => auditLog.CorrelationId)
            .HasMaxLength(100);

        builder.Property(auditLog => auditLog.IpAddress)
            .HasMaxLength(100);

        builder.Property(auditLog => auditLog.UserAgent)
            .HasMaxLength(500);

        builder.Property(auditLog => auditLog.OccurredAtUtc)
            .IsRequired();

        builder.Property(auditLog => auditLog.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(auditLog => auditLog.OrganizationId);

        builder.HasIndex(auditLog => new
        {
            auditLog.OrganizationId,
            auditLog.OccurredAtUtc
        });

        builder.HasIndex(auditLog => new
        {
            auditLog.EntityName,
            auditLog.EntityId
        });

        builder.HasIndex(auditLog => auditLog.UserId);
    }
}
