import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Cáritas Brigadas de Salud",
  description:
    "Panel web institucional para brigadas de salud de Cáritas de Monterrey.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es">
      <body>{children}</body>
    </html>
  );
}
