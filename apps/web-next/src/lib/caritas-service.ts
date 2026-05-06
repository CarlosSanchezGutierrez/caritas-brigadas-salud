import { getApiAuthHeaders } from "@/lib/auth-headers";
import { caritasApiGet } from "@/lib/caritas-api";
import { API_BASE_URL, DEV_ORGANIZATION_ID } from "@/lib/config";
import type { AuditLogSummary, HealthStatus, ReportSummary } from "@/types/api";

export async function getSystemHealth(): Promise<HealthStatus | null> {
  const response = await caritasApiGet<HealthStatus>("/health");
  return response.data ?? null;
}

export async function getReportSummary(): Promise<ReportSummary | null> {
  const response = await caritasApiGet<ReportSummary>(
    `/organizations/${DEV_ORGANIZATION_ID}/reports/summary`,
  );

  return response.data ?? null;
}

export async function getAuditLogs(): Promise<AuditLogSummary[]> {
  const response = await caritasApiGet<AuditLogSummary[]>(
    `/organizations/${DEV_ORGANIZATION_ID}/audit-logs`,
  );

  return Array.isArray(response.data) ? response.data : [];
}

export async function downloadReportSummaryCsv(): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/organizations/${DEV_ORGANIZATION_ID}/reports/summary.csv`,
    {
      method: "GET",
      headers: {
        Accept: "text/csv",
        ...getApiAuthHeaders(),
      },
      cache: "no-store",
    },
  );

  if (!response.ok) {
    throw new Error(`CSV export failed with HTTP ${response.status}.`);
  }

  const blob = await response.blob();
  const url = window.URL.createObjectURL(blob);
  const anchor = document.createElement("a");

  anchor.href = url;
  anchor.download = "caritas-report-summary.csv";
  document.body.appendChild(anchor);
  anchor.click();
  anchor.remove();

  window.URL.revokeObjectURL(url);
}
