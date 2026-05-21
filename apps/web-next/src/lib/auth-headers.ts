import { AUTH_MODE } from "@/lib/config";
import { getDevelopmentAuthHeaders } from "@/lib/dev-auth";

const OIDC_ACCESS_TOKEN_STORAGE_KEYS = [
  "caritas.oidc.accessToken",
  "caritas_oidc_access_token",
] as const;

export function getApiAuthHeaders(accessToken?: string): Record<string, string> {
  if (AUTH_MODE === "development") {
    return getDevelopmentAuthHeaders();
  }

  if (AUTH_MODE === "oidc") {
    const token = normalizeBearerToken(
      accessToken ?? readOidcAccessTokenFromBrowserStorage(),
    );

    if (token === null) {
      throw new Error(
        "OIDC access token is required before calling protected API endpoints.",
      );
    }

    return {
      Authorization: `Bearer ${token}`,
    };
  }

  return {} satisfies Record<string, string>;
}

function readOidcAccessTokenFromBrowserStorage(): string | null {
  if (typeof window === "undefined") {
    return null;
  }

  const storageCandidates = [window.sessionStorage, window.localStorage];

  for (const storage of storageCandidates) {
    for (const key of OIDC_ACCESS_TOKEN_STORAGE_KEYS) {
      try {
        const value = storage.getItem(key);

        if (value !== null && value.trim().length > 0) {
          return value;
        }
      } catch {
        continue;
      }
    }
  }

  return null;
}

function normalizeBearerToken(value: string | null | undefined): string | null {
  if (value === null || value === undefined) {
    return null;
  }

  const trimmed = value.trim();

  if (trimmed.length === 0) {
    return null;
  }

  if (trimmed.toLowerCase().startsWith("bearer ")) {
    const tokenOnly = trimmed.slice("bearer ".length).trim();
    return tokenOnly.length === 0 ? null : tokenOnly;
  }

  return trimmed;
}