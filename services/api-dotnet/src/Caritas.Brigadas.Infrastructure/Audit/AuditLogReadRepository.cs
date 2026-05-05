using System.Data;
using System.Data.Common;
using Caritas.Brigadas.Application.Audit;
using Caritas.Brigadas.Contracts.Audit;
using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Caritas.Brigadas.Infrastructure.Audit;

public sealed class AuditLogReadRepository : IAuditLogReadRepository
{
    private static readonly string[] CandidateTableNames =
    {
        "AuditLogs",
        "AuditEntries",
        "AuditEvents"
    };

    private readonly CaritasDbContext _dbContext;

    public AuditLogReadRepository(CaritasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AuditLogSummaryDto>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var table = await FindAuditTableAsync(cancellationToken);

        if (table is null)
        {
            return Array.Empty<AuditLogSummaryDto>();
        }

        var columns = await GetColumnsAsync(table.Value, cancellationToken);

        if (!columns.Contains("OrganizationId"))
        {
            return Array.Empty<AuditLogSummaryDto>();
        }

        var orderColumn = PickFirstExisting(
            columns,
            "OccurredAtUtc",
            "TimestampUtc",
            "CreatedAt",
            "Id");

        var sql = $"""
            SELECT TOP (200) *
            FROM [{table.Value.SchemaName}].[{table.Value.TableName}]
            WHERE [OrganizationId] = @organizationId
            ORDER BY [{orderColumn}] DESC
            """;

        var rows = await ExecuteRowsAsync(
            sql,
            command =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@organizationId";
                parameter.Value = organizationId;
                command.Parameters.Add(parameter);
            },
            cancellationToken);

        return rows
            .Select(MapToDto)
            .ToArray();
    }

    public async Task<AuditLogSummaryDto?> GetByIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default)
    {
        var table = await FindAuditTableAsync(cancellationToken);

        if (table is null)
        {
            return null;
        }

        var columns = await GetColumnsAsync(table.Value, cancellationToken);

        if (!columns.Contains("Id"))
        {
            return null;
        }

        var sql = $"""
            SELECT TOP (1) *
            FROM [{table.Value.SchemaName}].[{table.Value.TableName}]
            WHERE [Id] = @auditLogId
            """;

        var rows = await ExecuteRowsAsync(
            sql,
            command =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@auditLogId";
                parameter.Value = auditLogId;
                command.Parameters.Add(parameter);
            },
            cancellationToken);

        return rows
            .Select(MapToDto)
            .SingleOrDefault();
    }

    private async Task<AuditTableInfo?> FindAuditTableAsync(
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP (1) TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
              AND TABLE_NAME IN ('AuditLogs', 'AuditEntries', 'AuditEvents')
            ORDER BY
              CASE TABLE_NAME
                WHEN 'AuditLogs' THEN 1
                WHEN 'AuditEntries' THEN 2
                WHEN 'AuditEvents' THEN 3
                ELSE 99
              END
            """;

        await using var connection = _dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new AuditTableInfo(
            reader.GetString(0),
            reader.GetString(1));
    }

    private async Task<IReadOnlySet<string>> GetColumnsAsync(
        AuditTableInfo table,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @schemaName
              AND TABLE_NAME = @tableName
            """;

        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var connection = _dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var schemaParameter = command.CreateParameter();
        schemaParameter.ParameterName = "@schemaName";
        schemaParameter.Value = table.SchemaName;
        command.Parameters.Add(schemaParameter);

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "@tableName";
        tableParameter.Value = table.TableName;
        command.Parameters.Add(tableParameter);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(reader.GetString(0));
        }

        return columns;
    }

    private async Task<IReadOnlyCollection<IReadOnlyDictionary<string, object?>>> ExecuteRowsAsync(
        string sql,
        Action<DbCommand> configure,
        CancellationToken cancellationToken)
    {
        var rows = new List<IReadOnlyDictionary<string, object?>>();

        await using var connection = _dbContext.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        configure(command);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < reader.FieldCount; index++)
            {
                row[reader.GetName(index)] = await reader.IsDBNullAsync(index, cancellationToken)
                    ? null
                    : reader.GetValue(index);
            }

            rows.Add(row);
        }

        return rows;
    }

    private static AuditLogSummaryDto MapToDto(
        IReadOnlyDictionary<string, object?> row)
    {
        return new AuditLogSummaryDto
        {
            Id = GetGuid(row, "Id", "AuditLogId", "AuditEntryId", "AuditEventId") ?? Guid.Empty,
            OrganizationId = GetGuid(row, "OrganizationId"),
            EntityName = GetString(row, "EntityName", "EntityType", "AggregateName", "TableName", "Resource") ?? string.Empty,
            EntityId = GetGuid(row, "EntityId", "AggregateId", "RecordId", "ResourceId"),
            Action = GetString(row, "Action", "ActionType", "EventType", "Operation") ?? string.Empty,
            UserId = GetGuid(row, "UserId", "ActorUserId", "CreatedByUserId", "PerformedByUserId"),
            OccurredAtUtc = GetDateTimeOffset(row, "OccurredAtUtc", "TimestampUtc", "CreatedAt", "AtUtc"),
            CorrelationId = GetString(row, "CorrelationId", "TraceId"),
            IpAddress = GetString(row, "IpAddress", "RemoteIpAddress"),
            DetailsJson = GetString(row, "DetailsJson", "MetadataJson", "PayloadJson", "Message")
        };
    }

    private static string PickFirstExisting(
        IReadOnlySet<string> columns,
        params string[] candidates)
    {
        foreach (var candidate in candidates)
        {
            if (columns.Contains(candidate))
            {
                return candidate;
            }
        }

        return "Id";
    }

    private static Guid? GetGuid(
        IReadOnlyDictionary<string, object?> row,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            if (!row.TryGetValue(key, out var value) || value is null)
            {
                continue;
            }

            if (value is Guid guid)
            {
                return guid;
            }

            if (Guid.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return null;
    }

    private static string? GetString(
        IReadOnlyDictionary<string, object?> row,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            if (row.TryGetValue(key, out var value) && value is not null)
            {
                var text = value.ToString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
        }

        return null;
    }

    private static DateTimeOffset? GetDateTimeOffset(
        IReadOnlyDictionary<string, object?> row,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            if (!row.TryGetValue(key, out var value) || value is null)
            {
                continue;
            }

            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            if (value is DateTime dateTime)
            {
                return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
            }

            if (DateTimeOffset.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return null;
    }

    private readonly record struct AuditTableInfo(
        string SchemaName,
        string TableName);
}
