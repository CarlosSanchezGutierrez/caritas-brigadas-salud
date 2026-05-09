import { expect, test } from "@playwright/test";

async function mockCaritasApi(page: import("@playwright/test").Page) {
  await page.route("**/api/v1/health", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        success: true,
        data: {
          service: "caritas-brigadas-api",
          status: "healthy",
          timestampUtc: "2026-01-01T00:00:00Z",
        },
      }),
    });
  });

  await page.route("**/api/v1/organizations/**/reports/summary", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        success: true,
        data: {
          totalPatients: 12,
          totalVisits: 18,
          totalServiceEncounters: 24,
        },
      }),
    });
  });

  await page.route("**/api/v1/organizations/**/audit-logs", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        success: true,
        data: { items: [
          {
            id: "audit-1",
            action: "reports.read",
            entityName: "ReportSummary",
            userId: "e2e-user",
            occurredAtUtc: "2026-01-01T00:00:00Z",
          },
        ] },
      }),
    });
  });

  await page.route("**/api/v1/organizations/**/reports/summary.csv", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "text/csv",
      body: "metric,value\\ntotalPatients,12\\n",
    });
  });
}

test.beforeEach(async ({ page }) => {
  await mockCaritasApi(page);
});

test("institutional app shell renders dashboard and primary navigation", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("heading", { name: /Dashboard institucional/i })).toBeVisible();
  await expect(page.getByRole("link", { name: /Dashboard/i })).toBeVisible();
  await expect(page.getByRole("link", { name: /Reportes/i })).toBeVisible();
  await expect(page.getByRole("link", { name: /Auditor/i })).toBeVisible();
  await expect(page.getByRole("link", { name: /Sistema/i })).toBeVisible();
  await expect(page.getByText(/healthy/i)).toBeVisible();
});

test("reports page renders report summary and CSV export action", async ({ page }) => {
  await page.goto("/reports");

  await expect(page.getByRole("heading", { name: /Reportes/i })).toBeVisible();
  await expect(page.getByRole("button", { name: /Exportar CSV/i })).toBeVisible();
  await expect(page.getByText(/totalPatients/i)).toBeVisible();
});

test("audit logs page renders audit trail without exposing sensitive payloads", async ({ page }) => {
  await page.goto("/audit-logs");

  await expect(page.getByRole("heading", { name: /Auditor/i })).toBeVisible();
  await expect(page.getByText(/reports.read/i)).toBeVisible();
  await expect(page.getByText(/ReportSummary/i)).toBeVisible();
});

test("system page renders security baseline information", async ({ page }) => {
  await page.goto("/system");

  await expect(page.getByRole("heading", { name: /Sistema/i })).toBeVisible();
  await expect(page.getByText(/Security baseline/i)).toBeVisible();
  await expect(page.getByText(/Health payload/i)).toBeVisible();
});
