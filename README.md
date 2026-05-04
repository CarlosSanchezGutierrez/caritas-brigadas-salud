# Cáritas Brigadas de Salud

Plataforma institucional para registrar, sincronizar, auditar, analizar y reportar atenciones realizadas en brigadas de salud de Cáritas de Monterrey.

Este repositorio es específico para Cáritas de Monterrey y el Tec de Monterrey. No es un SaaS comercial, no es una plataforma multi-ONG pública y no debe mezclarse con proyectos comerciales externos.

## Stack objetivo

- Web/PWA: Next.js + TypeScript
- iOS/iPadOS: SwiftUI
- Android: Flutter
- Backend: ASP.NET Core
- Base de datos: SQL Server
- Contratos: OpenAPI + JSON Schema
- Formularios: JSON versionados
- Offline: almacenamiento local cifrado + sincronización por lotes
- Seguridad: RBAC, auditoría, cifrado, rate limiting y control de dispositivos

## Principios no negociables

- Las apps nunca se conectan directo a SQL Server.
- Web, iOS y Android consumen la misma API.
- El sistema debe funcionar offline en campo.
- Los alumnos no deben acceder a datos reales identificables.
- Toda acción sensible debe quedar auditada.
- La IA solo puede usarse para apoyo administrativo, analítico o documental.
- No se permite IA para diagnóstico, tratamiento o decisión clínica autónoma.

## Estado del proyecto

Fase inicial: estructura institucional, documentación técnica y primer vertical slice.
