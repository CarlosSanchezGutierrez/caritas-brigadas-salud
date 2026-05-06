export type WebAuthMode = "development" | "oidc";

const rawAuthMode = process.env.NEXT_PUBLIC_AUTH_MODE ?? "development";

if (rawAuthMode !== "development" && rawAuthMode !== "oidc") {
  throw new Error(
    `Invalid NEXT_PUBLIC_AUTH_MODE value: ${rawAuthMode}. Expected development or oidc.`,
  );
}

export const AUTH_MODE: WebAuthMode = rawAuthMode;
export const IS_DEVELOPMENT_AUTH = AUTH_MODE === "development";
export const IS_OIDC_AUTH = AUTH_MODE === "oidc";

export const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://localhost:7044/api/v1";

export const DEV_ORGANIZATION_ID =
  process.env.NEXT_PUBLIC_DEV_ORGANIZATION_ID ??
  "4df92032-4a1c-4cf2-b48f-15b570cd073a";

export const DEV_USER_ID =
  process.env.NEXT_PUBLIC_DEV_USER_ID ??
  "76279895-817d-47d2-b5c2-2a1e306db4f9";

export const DEV_USER_NAME =
  process.env.NEXT_PUBLIC_DEV_USER_NAME ?? "Carlos Sanchez Gutierrez";

export const DEV_USER_EMAIL =
  process.env.NEXT_PUBLIC_DEV_USER_EMAIL ?? "carlos.test@caritas.local";
