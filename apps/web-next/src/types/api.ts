export type HealthStatus = {
  service?: string;
  status?: string;
  timestampUtc?: string;
};

export type ReportSummary = Record<string, unknown>;

export type AuditLogSummary = {
  id?: string;
  organizationId?: string;
  userId?: string | null;
  action?: string;
  entityName?: string;
  entityId?: string | null;
  occurredAtUtc?: string;
  createdAtUtc?: string;
};
