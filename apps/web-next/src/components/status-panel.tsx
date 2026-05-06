import type { ReactNode } from "react";

type StatusPanelProps = {
  title: string;
  state?: "ok" | "warning" | "error" | "neutral";
  children: ReactNode;
};

export function StatusPanel({
  title,
  state = "neutral",
  children,
}: StatusPanelProps) {
  return (
    <section className={`status-panel ${state}`}>
      <div className="status-dot" aria-hidden="true" />
      <div>
        <h2>{title}</h2>
        <div>{children}</div>
      </div>
    </section>
  );
}
