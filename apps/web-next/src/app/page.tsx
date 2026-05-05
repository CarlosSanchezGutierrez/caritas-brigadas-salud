"use client";

import { useEffect, useState } from "react";
import { caritasApiGet } from "@/lib/caritas-api";
import { API_BASE_URL, DEV_ORGANIZATION_ID } from "@/lib/config";
import type { AuditLogSummary, HealthStatus, ReportSummary } from "@/types/api";

type LoadState = {
  health: HealthStatus | null;
  reportSummary: ReportSummary | null;
  auditLogs: AuditLogSummary[];
  error: string | null;
  isLoading: boolean;
};

const initialState: LoadState = {
  health: null,
  reportSummary: null,
  auditLogs: [],
  error: null,
  isLoading: true,
};

export default function HomePage() {
  const [state, setState] = useState<LoadState>(initialState);

  useEffect(() => {
    async function loadDashboardData() {
      try {
        const [healthResponse, reportResponse, auditResponse] =
          await Promise.all([
            caritasApiGet<HealthStatus>("/health"),
            caritasApiGet<ReportSummary>(
              `/organizations/${DEV_ORGANIZATION_ID}/reports/summary`,
            ),
            caritasApiGet<AuditLogSummary[]>(
              `/organizations/${DEV_ORGANIZATION_ID}/audit-logs`,
            ),
          ]);

        setState({
          health: healthResponse.data ?? null,
          reportSummary: reportResponse.data ?? null,
          auditLogs: auditResponse.data ?? [],
          error: null,
          isLoading: false,
        });
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "No se pudo cargar la información del backend.";

        setState({
          ...initialState,
          error: message,
          isLoading: false,
        });
      }
    }

    void loadDashboardData();
  }, []);

  return (
    <main className="page-shell">
      <section className="hero">
        <div>
          <p className="eyebrow">Cáritas Brigadas de Salud</p>
          <h1>Panel web institucional</h1>
          <p className="hero-copy">
            Primer scaffold web conectado al backend ASP.NET Core local. Este
            panel todavía usa autenticación de desarrollo por headers y no debe
            usarse con datos reales.
          </p>
        </div>

        <div className="status-card">
          <span className="status-label">API base URL</span>
          <strong>{API_BASE_URL}</strong>
        </div>
      </section>

      {state.error ? (
        <section className="error-panel">
          <h2>No se pudo conectar con la API</h2>
          <p>{state.error}</p>
          <p>
            Verifica que el backend esté corriendo en{" "}
            <code>http://localhost:5031</code>.
          </p>
        </section>
      ) : null}

      <section className="grid">
        <article className="card">
          <div className="card-header">
            <span>Health</span>
            <strong>{state.isLoading ? "Cargando" : "OK"}</strong>
          </div>
          <pre>{JSON.stringify(state.health, null, 2)}</pre>
        </article>

        <article className="card">
          <div className="card-header">
            <span>Reports summary</span>
            <strong>{state.reportSummary ? "Disponible" : "Pendiente"}</strong>
          </div>
          <pre>{JSON.stringify(state.reportSummary, null, 2)}</pre>
        </article>

        <article className="card wide">
          <div className="card-header">
            <span>Audit logs</span>
            <strong>{state.auditLogs.length} eventos</strong>
          </div>

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
                {state.auditLogs.slice(0, 10).map((log, index) => (
                  <tr key={log.id ?? `${log.action}-${index}`}>
                    <td>{log.action ?? "N/A"}</td>
                    <td>{log.entityName ?? "N/A"}</td>
                    <td>{log.occurredAtUtc ?? "N/A"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </article>
      </section>
    </main>
  );
}
