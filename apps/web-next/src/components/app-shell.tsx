"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import type { ReactNode } from "react";
import { APP_NAVIGATION } from "@/lib/app-navigation";

type AppShellProps = {
  eyebrow?: string;
  title: string;
  subtitle: string;
  children: ReactNode;
  actions?: ReactNode;
};

export function AppShell({
  eyebrow = "Cáritas Brigadas de Salud",
  title,
  subtitle,
  children,
  actions,
}: AppShellProps) {
  const pathname = usePathname();

  return (
    <div className="app-shell">
      <aside className="app-sidebar">
        <div className="brand-block">
          <div className="brand-mark">CB</div>
          <div>
            <strong>Cáritas</strong>
            <span>Brigadas de Salud</span>
          </div>
        </div>

        <nav className="app-nav" aria-label="Navegación principal">
          {APP_NAVIGATION.map((item) => {
            const isActive =
              item.href === "/" ? pathname === "/" : pathname.startsWith(item.href);

            return (
              <Link
                key={item.href}
                href={item.href}
                className={isActive ? "nav-item active" : "nav-item"}
              >
                <span>
                  <strong>{item.label}</strong>
                  <small>{item.description}</small>
                </span>
                {item.badge ? <em>{item.badge}</em> : null}
              </Link>
            );
          })}
        </nav>

        <div className="sidebar-footer">
          <span>Modo actual</span>
          <strong>Development headers</strong>
          <small>Autenticación real pendiente para una fase posterior.</small>
        </div>
      </aside>

      <section className="app-main">
        <header className="app-topbar">
          <div>
            <p className="eyebrow">{eyebrow}</p>
            <h1>{title}</h1>
            <p>{subtitle}</p>
          </div>
          {actions ? <div className="topbar-actions">{actions}</div> : null}
        </header>

        {children}
      </section>
    </div>
  );
}
