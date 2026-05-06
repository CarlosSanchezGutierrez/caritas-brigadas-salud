"use client";

import { useEffect, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { EmptyState } from "@/components/empty-state";
import { MetricCard } from "@/components/metric-card";
import { getAuditLogs } from "@/lib/caritas-service";
import type { AuditLogSummary } from "@/types/api";

type AuditState = {
  logs: AuditLogSummary[];
  isLoading: boolean;
  error: string | null;
};

export default function AuditLogsPage() {
  const [state, setState] = useState<AuditState>({
    logs: [],
    isLoading: true,
    error: null,
  });

  useEffect(() => {
    async function loadAuditLogs() {
      try {
        const logs = await getAuditLogs();

        setState({
          logs,
          isLoading: false,
          error: null,
        });
      } catch (error) {
        setState({
          logs: [],
          isLoading: false,
          error:
            error instanceof Error
              ? error.message
              : "No se pudieron cargar los eventos de auditoría.",
        });
      }
    }

    void loadAuditLogs();
  }, []);

  return (
    <AppShell
      title="Auditoría"
      subtitle="Bitácora de eventos relevantes para trazabilidad, control interno y revisión técnica."
    >
      {state.error ? (
        <section className="error-panel">
          <h2>Error al cargar auditoría</h2>
          <p>{state.error}</p>
        </section>
      ) : null}

      <section className="metric-grid">
        <MetricCard
          label="Eventos"
          value={state.isLoading ? "..." : state.logs.length}
          description="Registros recuperados desde audit logs."
        />
        <MetricCard
          label="Trazabilidad"
          value="Activa"
          description="Base para control institucional y revisión técnica."
        />
        <MetricCard
          label="Datos sensibles"
          value="No UI"
          description="La interfaz evita exponer payloads sensibles."
        />
        <MetricCard
          label="Modo"
          value="Lectura"
          description="Pantalla enfocada solo en consulta."
        />
      </section>

      <article className="card wide">
        <div className="card-header">
          <div>
            <span>Audit</span>
            <h2>Eventos recientes</h2>
          </div>
        </div>

        {state.logs.length === 0 ? (
          <EmptyState
            title={state.isLoading ? "Cargando eventos" : "Sin eventos"}
            description="Los eventos de auditoría aparecerán aquí cuando existan registros."
          />
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Acción</th>
                  <th>Entidad</th>
                  <th>Usuario</th>
                  <th>Fecha UTC</th>
                </tr>
              </thead>
              <tbody>
                {state.logs.map((log, index) => (
                  <tr key={log.id ?? `${log.action}-${index}`}>
                    <td>{log.action ?? "N/A"}</td>
                    <td>{log.entityName ?? "N/A"}</td>
                    <td>{log.userId ?? "Sistema"}</td>
                    <td>{log.occurredAtUtc ?? log.createdAtUtc ?? "N/A"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </article>
    </AppShell>
  );
}
