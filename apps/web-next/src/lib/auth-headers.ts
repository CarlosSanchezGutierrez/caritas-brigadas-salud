import { AUTH_MODE } from "@/lib/config";
import { getDevelopmentAuthHeaders } from "@/lib/dev-auth";

export function getApiAuthHeaders(): Record<string, string> {
  if (AUTH_MODE === "development") {
    return getDevelopmentAuthHeaders();
  }

  if (AUTH_MODE === "oidc") {
    return {};
  }

  return {} satisfies Record<string, string>;
}
