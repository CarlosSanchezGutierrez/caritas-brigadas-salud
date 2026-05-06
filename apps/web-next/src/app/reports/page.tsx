"use client";

import { useEffect, useMemo, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { EmptyState } from "@/components/empty-state";
import { MetricCard } from "@/components/metric-card";
import {
  downloadReportSummaryCsv,
  getReportSummary,
} from "@/lib/caritas-service";
import type { ReportSummary } from "@/types/api";

type ReportsState = {
  data: ReportSummary | null;
  isLoading: boolean;
  isExporting: boolean;
  error: string | null;
};

export default function ReportsPage() {
  const [state, setState] = useState<ReportsState>({
    data: null,
    isLoading: true,
    isExporting: false,
    error: null,
  });

  useEffect(() => {
    async function loadReports() {
      try {
        const data = await getReportSummary();

        setState({
          data,
          isLoading: false,
          isExporting: false,
          error: null,
        });
      } catch (error) {
        setState({
          data: null,
          isLoading: false,
          isExporting: false,
          error:
            error instanceof Error
              ? error.message
              : "No se pudieron cargar los reportes.",
        });
      }
    }

    void loadReports();
  }, []);

  const entries = useMemo(() => {
    return state.data ? Object.entries(state.data) : [];
  }, [state.data]);

  async function handleExportCsv() {
    try {
      setState((current) => ({ ...current, isExporting: true }));
      await downloadReportSummaryCsv();
      setState((current) => ({ ...current, isExporting: false }));
    } catch (error) {
      setState((current) => ({
        ...current,
        isExporting: false,
        error:
          error instanceof Error
            ? error.message
            : "No se pudo exportar el CSV.",
      }));
    }
  }

  return (
    <AppShell
      title="Reportes"
      subtitle="Consulta y exportación de indicadores institucionales para seguimiento operativo."
      actions={
        <button
          className="primary-button"
          type="button"
          onClick={handleExportCsv}
          disabled={state.isExporting}
        >
          {state.isExporting ? "Exportando..." : "Exportar CSV"}
        </button>
      }
    >
      {state.error ? (
        <section className="error-panel">
          <h2>Error al cargar reportes</h2>
          <p>{state.error}</p>
        </section>
      ) : null}

      <section className="metric-grid">
        <MetricCard
          label="Campos"
          value={state.isLoading ? "..." : entries.length}
          description="Cantidad de métricas devueltas por el backend."
        />
        <MetricCard
          label="Exportación"
          value="CSV"
          description="Formato compatible con procesos institucionales."
        />
        <MetricCard
          label="Fuente"
          value="API"
          description="Datos consumidos desde ASP.NET Core."
        />
        <MetricCard
          label="Modo"
          value="Dev"
          description="Acceso con headers de desarrollo."
        />
      </section>

      <article className="card wide">
        <div className="card-header">
          <div>
            <span>Summary</span>
            <h2>Datos del reporte</h2>
          </div>
        </div>

        {entries.length === 0 ? (
          <EmptyState
            title={state.isLoading ? "Cargando reporte" : "Sin datos"}
            description="El summary aparecerá aquí cuando el backend devuelva información."
          />
        ) : (
          <div className="table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Métrica</th>
                  <th>Valor</th>
                </tr>
              </thead>
              <tbody>
                {entries.map(([key, value]) => (
                  <tr key={key}>
                    <td>{key}</td>
                    <td>{JSON.stringify(value)}</td>
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
