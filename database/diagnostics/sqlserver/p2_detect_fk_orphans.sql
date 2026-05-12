/*
P2 FK orphan detection script
Target: SQL Server
Purpose: run before applying P2 FK migrations to a real database.
This script is read-only and does not modify data.
*/

SET NOCOUNT ON;

CREATE TABLE #p2_fk_orphan_results
(
    check_name nvarchar(250) NOT NULL,
    dependent_table nvarchar(250) NOT NULL,
    dependent_column nvarchar(128) NOT NULL,
    principal_table nvarchar(250) NOT NULL,
    principal_column nvarchar(128) NOT NULL,
    is_required bit NOT NULL,
    orphan_count bigint NOT NULL
);

INSERT INTO #p2_fk_orphan_results
SELECT 'Role.OrganizationId -> Organization.Id', 'core.roles', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [core].[roles] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'User.OrganizationId -> Organization.Id', 'core.users', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [core].[users] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'UserRole.OrganizationId -> Organization.Id', 'core.user_roles', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [core].[user_roles] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'UserRole.UserId -> User.Id', 'core.user_roles', 'UserId', 'core.users', 'Id', 1, COUNT_BIG(*)
FROM [core].[user_roles] d
LEFT JOIN [core].[users] p ON p.[Id] = d.[UserId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'UserRole.RoleId -> Role.Id', 'core.user_roles', 'RoleId', 'core.roles', 'Id', 1, COUNT_BIG(*)
FROM [core].[user_roles] d
LEFT JOIN [core].[roles] p ON p.[Id] = d.[RoleId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'RolePermission.RoleId -> Role.Id', 'core.role_permissions', 'RoleId', 'core.roles', 'Id', 1, COUNT_BIG(*)
FROM [core].[role_permissions] d
LEFT JOIN [core].[roles] p ON p.[Id] = d.[RoleId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'RolePermission.PermissionId -> Permission.Id', 'core.role_permissions', 'PermissionId', 'core.permissions', 'Id', 1, COUNT_BIG(*)
FROM [core].[role_permissions] d
LEFT JOIN [core].[permissions] p ON p.[Id] = d.[PermissionId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Service.OrganizationId -> Organization.Id', 'core.services', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [core].[services] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Community.OrganizationId -> Organization.Id', 'brigades.communities', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [brigades].[communities] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MobileUnit.OrganizationId -> Organization.Id', 'brigades.mobile_units', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [brigades].[mobile_units] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Brigade.OrganizationId -> Organization.Id', 'brigades.brigades', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [brigades].[brigades] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Brigade.CommunityId -> Community.Id', 'brigades.brigades', 'CommunityId', 'brigades.communities', 'Id', 0, COUNT_BIG(*)
FROM [brigades].[brigades] d
LEFT JOIN [brigades].[communities] p ON p.[Id] = d.[CommunityId]
WHERE d.[CommunityId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Brigade.MobileUnitId -> MobileUnit.Id', 'brigades.brigades', 'MobileUnitId', 'brigades.mobile_units', 'Id', 0, COUNT_BIG(*)
FROM [brigades].[brigades] d
LEFT JOIN [brigades].[mobile_units] p ON p.[Id] = d.[MobileUnitId]
WHERE d.[MobileUnitId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'BrigadeService.BrigadeId -> Brigade.Id', 'brigades.brigade_services', 'BrigadeId', 'brigades.brigades', 'Id', 1, COUNT_BIG(*)
FROM [brigades].[brigade_services] d
LEFT JOIN [brigades].[brigades] p ON p.[Id] = d.[BrigadeId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'BrigadeService.ServiceId -> Service.Id', 'brigades.brigade_services', 'ServiceId', 'core.services', 'Id', 1, COUNT_BIG(*)
FROM [brigades].[brigade_services] d
LEFT JOIN [core].[services] p ON p.[Id] = d.[ServiceId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'Patient.OrganizationId -> Organization.Id', 'clinical.patients', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[patients] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'PatientGuardian.PatientId -> Patient.Id', 'clinical.patient_guardians', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[patient_guardians] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'PatientVisit.OrganizationId -> Organization.Id', 'clinical.patient_visits', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[patient_visits] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'PatientVisit.PatientId -> Patient.Id', 'clinical.patient_visits', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[patient_visits] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'PatientVisit.BrigadeId -> Brigade.Id', 'clinical.patient_visits', 'BrigadeId', 'brigades.brigades', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[patient_visits] d
LEFT JOIN [brigades].[brigades] p ON p.[Id] = d.[BrigadeId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'ServiceEncounter.OrganizationId -> Organization.Id', 'clinical.service_encounters', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[service_encounters] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'ServiceEncounter.PatientId -> Patient.Id', 'clinical.service_encounters', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[service_encounters] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'ServiceEncounter.VisitId -> PatientVisit.Id', 'clinical.service_encounters', 'VisitId', 'clinical.patient_visits', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[service_encounters] d
LEFT JOIN [clinical].[patient_visits] p ON p.[Id] = d.[VisitId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'ServiceEncounter.BrigadeId -> Brigade.Id', 'clinical.service_encounters', 'BrigadeId', 'brigades.brigades', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[service_encounters] d
LEFT JOIN [brigades].[brigades] p ON p.[Id] = d.[BrigadeId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'ServiceEncounter.ServiceId -> Service.Id', 'clinical.service_encounters', 'ServiceId', 'core.services', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[service_encounters] d
LEFT JOIN [core].[services] p ON p.[Id] = d.[ServiceId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicalReferral.OrganizationId -> Organization.Id', 'clinical.medical_referrals', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medical_referrals] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicalReferral.PatientId -> Patient.Id', 'clinical.medical_referrals', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medical_referrals] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicalReferral.EncounterId -> ServiceEncounter.Id', 'clinical.medical_referrals', 'EncounterId', 'clinical.service_encounters', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medical_referrals] d
LEFT JOIN [clinical].[service_encounters] p ON p.[Id] = d.[EncounterId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicationDelivery.OrganizationId -> Organization.Id', 'clinical.medication_deliveries', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medication_deliveries] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicationDelivery.PatientId -> Patient.Id', 'clinical.medication_deliveries', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medication_deliveries] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MedicationDelivery.EncounterId -> ServiceEncounter.Id', 'clinical.medication_deliveries', 'EncounterId', 'clinical.service_encounters', 'Id', 1, COUNT_BIG(*)
FROM [clinical].[medication_deliveries] d
LEFT JOIN [clinical].[service_encounters] p ON p.[Id] = d.[EncounterId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'FormTemplate.OrganizationId -> Organization.Id', 'forms.form_templates', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [forms].[form_templates] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'FormTemplate.ServiceId -> Service.Id', 'forms.form_templates', 'ServiceId', 'core.services', 'Id', 1, COUNT_BIG(*)
FROM [forms].[form_templates] d
LEFT JOIN [core].[services] p ON p.[Id] = d.[ServiceId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'FormResponse.OrganizationId -> Organization.Id', 'forms.form_responses', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [forms].[form_responses] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'FormResponse.FormTemplateId -> FormTemplate.Id', 'forms.form_responses', 'FormTemplateId', 'forms.form_templates', 'Id', 1, COUNT_BIG(*)
FROM [forms].[form_responses] d
LEFT JOIN [forms].[form_templates] p ON p.[Id] = d.[FormTemplateId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'FormResponse.EncounterId -> ServiceEncounter.Id', 'forms.form_responses', 'EncounterId', 'clinical.service_encounters', 'Id', 1, COUNT_BIG(*)
FROM [forms].[form_responses] d
LEFT JOIN [clinical].[service_encounters] p ON p.[Id] = d.[EncounterId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentTemplate.OrganizationId -> Organization.Id', 'documents.document_templates', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [documents].[document_templates] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentTemplate.AppliesToServiceId -> Service.Id', 'documents.document_templates', 'AppliesToServiceId', 'core.services', 'Id', 0, COUNT_BIG(*)
FROM [documents].[document_templates] d
LEFT JOIN [core].[services] p ON p.[Id] = d.[AppliesToServiceId]
WHERE d.[AppliesToServiceId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentSignature.OrganizationId -> Organization.Id', 'documents.document_signatures', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [documents].[document_signatures] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentSignature.DocumentTemplateId -> DocumentTemplate.Id', 'documents.document_signatures', 'DocumentTemplateId', 'documents.document_templates', 'Id', 1, COUNT_BIG(*)
FROM [documents].[document_signatures] d
LEFT JOIN [documents].[document_templates] p ON p.[Id] = d.[DocumentTemplateId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentSignature.PatientId -> Patient.Id', 'documents.document_signatures', 'PatientId', 'clinical.patients', 'Id', 0, COUNT_BIG(*)
FROM [documents].[document_signatures] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE d.[PatientId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentSignature.VisitId -> PatientVisit.Id', 'documents.document_signatures', 'VisitId', 'clinical.patient_visits', 'Id', 0, COUNT_BIG(*)
FROM [documents].[document_signatures] d
LEFT JOIN [clinical].[patient_visits] p ON p.[Id] = d.[VisitId]
WHERE d.[VisitId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'DocumentSignature.EncounterId -> ServiceEncounter.Id', 'documents.document_signatures', 'EncounterId', 'clinical.service_encounters', 'Id', 0, COUNT_BIG(*)
FROM [documents].[document_signatures] d
LEFT JOIN [clinical].[service_encounters] p ON p.[Id] = d.[EncounterId]
WHERE d.[EncounterId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MediaRelease.OrganizationId -> Organization.Id', 'documents.media_releases', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [documents].[media_releases] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MediaRelease.PatientId -> Patient.Id', 'documents.media_releases', 'PatientId', 'clinical.patients', 'Id', 1, COUNT_BIG(*)
FROM [documents].[media_releases] d
LEFT JOIN [clinical].[patients] p ON p.[Id] = d.[PatientId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'MediaRelease.VisitId -> PatientVisit.Id', 'documents.media_releases', 'VisitId', 'clinical.patient_visits', 'Id', 0, COUNT_BIG(*)
FROM [documents].[media_releases] d
LEFT JOIN [clinical].[patient_visits] p ON p.[Id] = d.[VisitId]
WHERE d.[VisitId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'SyncBatch.OrganizationId -> Organization.Id', 'sync.sync_batches', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [sync].[sync_batches] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'SyncBatch.BrigadeId -> Brigade.Id', 'sync.sync_batches', 'BrigadeId', 'brigades.brigades', 'Id', 0, COUNT_BIG(*)
FROM [sync].[sync_batches] d
LEFT JOIN [brigades].[brigades] p ON p.[Id] = d.[BrigadeId]
WHERE d.[BrigadeId] IS NOT NULL AND p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'SyncEvent.OrganizationId -> Organization.Id', 'sync.sync_events', 'OrganizationId', 'core.organizations', 'Id', 1, COUNT_BIG(*)
FROM [sync].[sync_events] d
LEFT JOIN [core].[organizations] p ON p.[Id] = d.[OrganizationId]
WHERE p.[Id] IS NULL;

INSERT INTO #p2_fk_orphan_results
SELECT 'SyncEvent.SyncBatchId -> SyncBatch.Id', 'sync.sync_events', 'SyncBatchId', 'sync.sync_batches', 'Id', 1, COUNT_BIG(*)
FROM [sync].[sync_events] d
LEFT JOIN [sync].[sync_batches] p ON p.[Id] = d.[SyncBatchId]
WHERE p.[Id] IS NULL;

SELECT 'DEFERRED_DEVICE_REFERENCE' AS result_type, 'SyncBatch.DeviceId -> Device.Id' AS check_name, COUNT_BIG(*) AS rows_with_device_id FROM [sync].[sync_batches] WHERE [DeviceId] IS NOT NULL
UNION ALL
SELECT 'DEFERRED_DEVICE_REFERENCE', 'FormResponse.DeviceId -> Device.Id', COUNT_BIG(*) FROM [forms].[form_responses] WHERE [DeviceId] IS NOT NULL
UNION ALL
SELECT 'DEFERRED_DEVICE_REFERENCE', 'DocumentSignature.DeviceId -> Device.Id', COUNT_BIG(*) FROM [documents].[document_signatures] WHERE [DeviceId] IS NOT NULL;

SELECT
    'P2_FK_ORPHAN_SUMMARY' AS result_type,
    SUM(orphan_count) AS total_orphans,
    SUM(CASE WHEN is_required = 1 THEN orphan_count ELSE 0 END) AS required_fk_orphans,
    SUM(CASE WHEN is_required = 0 THEN orphan_count ELSE 0 END) AS optional_fk_orphans
FROM #p2_fk_orphan_results;

SELECT 'P2_FK_ORPHAN_DETAIL' AS result_type, check_name, dependent_table, dependent_column, principal_table, principal_column, is_required, orphan_count
FROM #p2_fk_orphan_results
WHERE orphan_count > 0
ORDER BY orphan_count DESC, check_name;

SELECT 'P2_FK_ORPHAN_ALL_CHECKS' AS result_type, check_name, dependent_table, dependent_column, principal_table, principal_column, is_required, orphan_count
FROM #p2_fk_orphan_results
ORDER BY check_name;

DROP TABLE #p2_fk_orphan_results;
