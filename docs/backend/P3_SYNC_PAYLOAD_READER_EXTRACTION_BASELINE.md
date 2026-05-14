# P3 Sync Payload Reader Extraction Baseline

Status: active  
Scope: extraction of sync event JSON object parsing and request deserialization from SyncBatchProcessor  
Target phase: P3-22C  
Depends on: P3-22B component extraction

---

## 1. Purpose

P3-22C extracts repeated sync payload parsing/validation from SyncBatchProcessor into SyncPayloadReader.

This package does not change domain behavior. It centralizes a repeated infrastructure concern:

- parse PayloadJson;
- require JSON object root;
- deserialize the request DTO;
- return a safe rejection reason when JSON is invalid;
- return a safe rejection reason when the request is null.

---

## 2. SyncPayloadReader contract

Rules:

- SyncPayloadReader must be an internal infrastructure sync component;
- SyncPayloadReader.TryReadObject must accept payload JSON, payload label, JsonSerializerOptions, typed request output, and rejection reason output;
- SyncPayloadReader must reject non-object JSON roots;
- SyncPayloadReader must reject invalid JSON;
- SyncPayloadReader must reject null deserialization results;
- SyncPayloadReader must not echo raw PayloadJson;
- SyncPayloadReader must not log raw PayloadJson;
- rejection reasons must be generic and safe.

---

## 3. SyncBatchProcessor contract

Rules:

- SyncBatchProcessor must use SyncPayloadReader.TryReadObject for all current create request DTOs;
- SyncBatchProcessor must use explicit typed out variables for current create request DTOs;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreatePatientRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreatePatientVisitRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateServiceEncounterRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateFormResponseRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateConsentDocumentRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateMedicalReferralRequest>;
- SyncBatchProcessor must not directly call JsonSerializer.Deserialize<CreateMedicationDeliveryRequest>;
- handler behavior must remain unchanged after successful payload parsing.

---

## 4. Non-negotiable constraints

Rules:

- no database migration;
- no endpoint contract change;
- no sync entity type expansion;
- no handler extraction in this package;
- no weakening of P3-21 integration hardening;
- no weakening of P3-22A zero technical debt gate;
- no weakening of P3-22B component extraction;
- no raw PayloadJson exposure;
- no raw clinical JSON echo in process results.

---

## 5. Acceptance criteria

P3-22C is complete when:

- SyncPayloadReader exists;
- SyncBatchProcessor uses SyncPayloadReader.TryReadObject for all current sync create DTOs;
- SyncBatchProcessor uses typed out variables for all current sync create DTOs;
- SyncBatchProcessor no longer directly deserializes current create DTOs;
- all P3 sync processor verifiers remain green;
- dotnet build and dotnet test remain green.
---

## 6. P3-22D formatting hygiene note

P3-22D removes formatting debt after payload reader extraction and makes SyncBatchProcessor formatting hygiene verifier-protected.
