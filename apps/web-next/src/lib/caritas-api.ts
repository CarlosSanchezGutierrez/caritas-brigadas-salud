import { getApiAuthHeaders } from "@/lib/auth-headers";
import { API_BASE_URL } from "@/lib/config";

export type ApiResponse<TData> = {
  success?: boolean;
  data?: TData;
  errorCode?: string;
  message?: string;
  traceId?: string;
  timestampUtc?: string;
  error?: unknown;
};

export class ApiClientError extends Error {
  public readonly status: number;
  public readonly responseBody: unknown;

  public constructor(status: number, responseBody: unknown) {
    super(`API request failed with HTTP ${status}.`);
    this.name = "ApiClientError";
    this.status = status;
    this.responseBody = responseBody;
  }
}

export async function caritasApiGet<TData>(
  path: string,
  options: RequestInit = {},
): Promise<ApiResponse<TData>> {
  const normalizedPath = path.startsWith("/") ? path : `/${path}`;
  const url = `${API_BASE_URL}${normalizedPath}`;

  const headers = new Headers(options.headers);
  headers.set("Accept", "application/json");

  for (const [key, value] of Object.entries(getApiAuthHeaders())) {
    headers.set(key, value);
  }

  const response = await fetch(url, {
    ...options,
    method: "GET",
    headers,
    cache: "no-store",
  });

  const contentType = response.headers.get("content-type") ?? "";
  const body = contentType.includes("application/json")
    ? await response.json()
    : await response.text();

  if (!response.ok) {
    throw new ApiClientError(response.status, body);
  }

  return body as ApiResponse<TData>;
}
