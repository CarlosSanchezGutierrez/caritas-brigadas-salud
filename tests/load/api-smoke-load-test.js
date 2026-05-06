import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  vus: Number(__ENV.K6_VUS ?? "5"),
  duration: __ENV.K6_DURATION ?? "1m",
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<750"],
  },
};

export const insecureSkipTLSVerify = true;

const baseUrl = __ENV.BASE_URL ?? "https://localhost:7044/api/v1";
const organizationId = __ENV.DEV_ORGANIZATION_ID ?? "4df92032-4a1c-4cf2-b48f-15b570cd073a";

const params = {
  headers: {
    Accept: "application/json",
    "X-Dev-User-Id": __ENV.DEV_USER_ID ?? "76279895-817d-47d2-b5c2-2a1e306db4f9",
    "X-Dev-Organization-Id": organizationId,
    "X-Dev-Roles": "SUPER_ADMIN",
    "X-Dev-Permissions": "reports.read,audit-logs.read,organizations.read",
    "X-Dev-Name": "Load Test User",
    "X-Dev-Email": "load.test@caritas.local",
  },
};

export default function () {
  const health = http.get(`${baseUrl}/health`, params);

  check(health, {
    "health status is 200": (response) => response.status === 200,
  });

  const reportSummary = http.get(`${baseUrl}/organizations/${organizationId}/reports/summary`, params);

  check(reportSummary, {
    "report summary is not server error": (response) => response.status < 500,
  });

  const auditLogs = http.get(`${baseUrl}/organizations/${organizationId}/audit-logs`, params);

  check(auditLogs, {
    "audit logs is not server error": (response) => response.status < 500,
  });

  sleep(1);
}
