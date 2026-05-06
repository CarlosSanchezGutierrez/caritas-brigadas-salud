import {
  DEV_ORGANIZATION_ID,
  DEV_USER_EMAIL,
  DEV_USER_ID,
  DEV_USER_NAME,
  IS_DEVELOPMENT_AUTH,
} from "@/lib/config";

const DEV_PERMISSIONS = [
  "organizations.read",
  "organizations.write",
  "users.read",
  "users.write",
  "roles.read",
  "roles.assign",
  "services.read",
  "services.seed",
  "communities.read",
  "communities.write",
  "mobile-units.read",
  "mobile-units.write",
  "brigades.read",
  "brigades.write",
  "brigade-services.read",
  "brigade-services.write",
  "patients.read",
  "patients.write",
  "patient-visits.read",
  "patient-visits.write",
  "service-encounters.read",
  "service-encounters.write",
  "form-templates.read",
  "form-templates.seed",
  "form-responses.read",
  "form-responses.write",
  "consent-documents.read",
  "consent-documents.write",
  "reports.read",
  "reports.export",
  "sync-batches.read",
  "sync-batches.write",
  "audit-logs.read",
];

export function getDevelopmentAuthHeaders(): Record<string, string> {
  if (!IS_DEVELOPMENT_AUTH) {
    return {};
  }

  return {
    "X-Dev-User-Id": DEV_USER_ID,
    "X-Dev-Organization-Id": DEV_ORGANIZATION_ID,
    "X-Dev-Roles": "SUPER_ADMIN",
    "X-Dev-Permissions": DEV_PERMISSIONS.join(","),
    "X-Dev-Name": DEV_USER_NAME,
    "X-Dev-Email": DEV_USER_EMAIL,
  };
}
