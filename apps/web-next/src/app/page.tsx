"use client";

import { useEffect, useMemo, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { EmptyState } from "@/components/empty-state";
import { MetricCard } from "@/components/metric-card";
import { StatusPanel } from "@/components/status-panel";
import {
  getAuditLogs,
  getReportSummary,
  getSystemHealth,
} from "@/lib/caritas-service";
import { API_BASE_URL, DEV_ORGANIZATION_ID } from "@/lib/config";
import type { AuditLogSummary, HealthStatus, ReportSummary } from "@/types/api";

type DashboardState = {
  health: HealthStatus | null;
  reportSummary: ReportSummary | null;
  auditLogs: AuditLogSummary[];
  isLoading: boolean;
  error: string | null;
};

const initialState: DashboardState = {
  health: null,
  reportSummary: null,
  auditLogs: [],
  isLoading: true,
  error: null,
};

export default function DashboardPage() {
  const [state, setState] = useState<DashboardState>(initialState);

  useEffect(() => {
    async function loadDashboard() {
      try {
        const [health, reportSummary, auditLogs] = await Promise.all([
          getSystemHealth(),
          getReportSummary(),
          getAuditLogs(),
        ]);

        setState({
          health,
          reportSummary,
          auditLogs,
          isLoading: false,
          error: null,
        });
      } catch (error) {
        setState({
          ...initialState,
          isLoading: false,
          error:
            error instanceof Error
              ? error.message
              : "No se pudo cargar el dashboard.",
        });
      }
    }

    void loadDashboard();
  }, []);

  const reportMetricCount = useMemo(() => {
    if (!state.reportSummary) {
      return 0;
    }

    return Object.keys(state.reportSummary).length;
  }, [state.reportSummary]);

  return (
    <AppShell
      title="Dashboard institucional"
      subtitle="Vista inicial para monitorear salud técnica, reportes y actividad reciente del sistema."
      actions={<span className="environment-pill">Development</span>}
    >
      {state.error ? (
        <StatusPanel title="No se pudo conectar con la API" state="error">
          <p>{state.error}</p>
          <p>
            Verifica que el backend esté disponible en <code>{API_BASE_URL}</code>.
          </p>
        </StatusPanel>
      ) : null}

      <section className="metric-grid">
        <MetricCard
          label="API"
          value={state.health?.status ?? (state.isLoading ? "Cargando" : "N/D")}
          description="Estado reportado por el endpoint de health."
        />
        <MetricCard
          label="Organización"
          value={DEV_ORGANIZATION_ID.slice(0, 8)}
          description="Contexto de desarrollo enviado por headers."
        />
        <MetricCard
          label="Reportes"
          value={state.isLoading ? "..." : reportMetricCount}
          description="Campos disponibles en el summary institucional."
        />
        <MetricCard
          label="Auditoría"
          value={state.isLoading ? "..." : state.auditLogs.length}
          description="Eventos recuperados desde el backend."
        />
      </section>

      <section className="content-grid">
        <article className="card">
          <div className="card-header">
            <div>
              <span>Health</span>
              <h2>Estado técnico</h2>
            </div>
          </div>
          <pre>{JSON.stringify(state.health, null, 2)}</pre>
        </article>

        <article className="card">
          <div className="card-header">
            <div>
              <span>Reports</span>
              <h2>Resumen institucional</h2>
            </div>
          </div>
          <pre>{JSON.stringify(state.reportSummary, null, 2)}</pre>
        </article>

        <article className="card wide">
          <div className="card-header">
            <div>
              <span>Audit logs</span>
              <h2>Actividad reciente</h2>
            </div>
          </div>

          {state.auditLogs.length === 0 ? (
            <EmptyState
              title="Sin eventos para mostrar"
              description="Cuando existan eventos de auditoría, aparecerán en esta sección."
            />
          ) : (
            <div className="table-wrapper">
              <table>
                <thead>
                  <tr>
                    <th>Acción</th>
                    <th>Entidad</th>
                    <th>Fecha UTC</th>
                  </tr>
                </thead>
                <tbody>
                  {state.auditLogs.slice(0, 8).map((log, index) => (
                    <tr key={log.id ?? `${log.action}-${index}`}>
                      <td>{log.action ?? "N/A"}</td>
                      <td>{log.entityName ?? "N/A"}</td>
                      <td>{log.occurredAtUtc ?? log.createdAtUtc ?? "N/A"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </article>
      </section>
    </AppShell>
  );
}
