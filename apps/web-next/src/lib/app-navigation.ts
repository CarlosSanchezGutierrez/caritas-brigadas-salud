export type AppNavigationItem = {
  href: string;
  label: string;
  description: string;
  badge?: string;
};

export const APP_NAVIGATION: AppNavigationItem[] = [
  {
    href: "/",
    label: "Dashboard",
    description: "Resumen operativo de brigadas, reportes y sistema.",
  },
  {
    href: "/reports",
    label: "Reportes",
    description: "Indicadores y exportación institucional.",
  },
  {
    href: "/audit-logs",
    label: "Auditoría",
    description: "Eventos relevantes del sistema.",
  },
  {
    href: "/system",
    label: "Sistema",
    description: "Estado técnico, seguridad y conectividad.",
    badge: "Dev",
  },
];
