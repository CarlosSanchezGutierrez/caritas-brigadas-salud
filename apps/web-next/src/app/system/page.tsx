"use client";

import { useEffect, useState } from "react";
import { AppShell } from "@/components/app-shell";
import { MetricCard } from "@/components/metric-card";
import { StatusPanel } from "@/components/status-panel";
import { getSystemHealth } from "@/lib/caritas-service";
import { API_BASE_URL } from "@/lib/config";
import type { HealthStatus } from "@/types/api";

export default function SystemPage() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function loadHealth() {
      try {
        const data = await getSystemHealth();
        setHealth(data);
      } catch (loadError) {
        setError(
          loadError instanceof Error
            ? loadError.message
            : "No se pudo consultar el estado del sistema.",
        );
      }
    }

    void loadHealth();
  }, []);

  return (
    <AppShell
      title="Sistema"
      subtitle="Estado técnico y checklist de seguridad para el entorno local de desarrollo."
      actions={<span className="environment-pill">Security baseline</span>}
    >
      <section className="metric-grid">
        <MetricCard
          label="API"
          value={health?.status ?? "N/D"}
          description="Estado reportado por el backend."
        />
        <MetricCard
          label="Base URL"
          value="Local"
          description={API_BASE_URL}
        />
        <MetricCard
          label="Headers"
          value="Activos"
          description="Security headers validados por smoke test."
        />
        <MetricCard
          label="CI"
          value="Verify"
          description="Build, tests y auditorías automatizadas."
        />
      </section>

      {error ? (
        <section className="error-panel">
          <h2>Error de conectividad</h2>
          <p>{error}</p>
        </section>
      ) : null}

      <section className="content-grid">
        <StatusPanel title="Hardening de API" state="ok">
          <p>Security headers, rate limiting y límites de request body fueron agregados al backend.</p>
        </StatusPanel>

        <StatusPanel title="Gates locales" state="ok">
          <p>verify-local y security-smoke-local validan build, tests, headers, auth y smoke funcional.</p>
        </StatusPanel>

        <StatusPanel title="Autenticación" state="warning">
          <p>La interfaz todavía usa headers de desarrollo. La autenticación real debe ser una fase separada.</p>
        </StatusPanel>

        <StatusPanel title="Datos reales" state="warning">
          <p>No usar datos sensibles reales hasta cerrar autenticación, control de sesiones y despliegue seguro.</p>
        </StatusPanel>
      </section>

      <article className="card wide">
        <div className="card-header">
          <div>
            <span>Health payload</span>
            <h2>Respuesta técnica</h2>
          </div>
        </div>
        <pre>{JSON.stringify(health, null, 2)}</pre>
      </article>
    </AppShell>
  );
}
