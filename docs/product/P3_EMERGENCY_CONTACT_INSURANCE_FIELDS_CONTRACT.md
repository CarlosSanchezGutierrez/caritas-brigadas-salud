# P3 Emergency Contact and Insurance Fields Contract

Status: active
Phase: P3-30C
Frontend readiness impact: BLOCKS_FULL_FRONTEND
Production readiness impact: BLOCKS_PRODUCTION_UNTIL_IMPLEMENTED_AND_EVIDENCED

---

## 1. Executive summary

This contract defines how emergency contact and insurance/social security information must be represented before frontend implementation.

The contract intentionally treats these fields as optional because medical brigades may serve patients who cannot provide complete contact or coverage information.

The frontend must not block care because emergency contact or insurance data is missing.

The backend must preserve explicit unavailable-data flags and reasons when these fields are not available.

---

## 2. Emergency contact fields

| Field | Type | Required | Notes |
|---|---|---:|---|
| hasEmergencyContact | bool | Yes | Indicates whether patient provided an emergency contact |
| emergencyContactFullName | string | Conditional | Required when hasEmergencyContact is true |
| emergencyContactPhoneNumber | string | Conditional | Required when hasEmergencyContact is true |
| emergencyContactRelationship | enum/string | Conditional | Required when hasEmergencyContact is true |
| emergencyContactNotes | string | No | Sensitive internal notes |
| emergencyContactIsUnavailable | bool | Yes | Indicates patient cannot provide contact |
| emergencyContactUnavailableReason | string | Conditional | Required when emergencyContactIsUnavailable is true |

---

## 3. Insurance and social security fields

| Field | Type | Required | Notes |
|---|---|---:|---|
| hasSocialSecurity | bool | Yes | Indicates whether patient reports public social security coverage |
| socialSecurityProvider | enum | Conditional | Recommended when hasSocialSecurity is true |
| socialSecurityProviderOther | string | Conditional | Required when socialSecurityProvider is OTHER |
| hasPrivateInsurance | bool | Yes | Indicates whether patient reports private insurance |
| privateInsuranceProvider | string | Conditional | Recommended when hasPrivateInsurance is true |
| insuranceCoverageNotes | string | No | Sensitive internal notes |
| insuranceInformationUnavailable | bool | Yes | Indicates information could not be provided |
| insuranceInformationUnavailableReason | string | Conditional | Required when insuranceInformationUnavailable is true |

MVP decision:

Do not collect national social security numbers or policy numbers unless Caritas explicitly confirms they are operationally necessary.

---

## 4. Emergency contact behavior

| Scenario | Expected behavior |
|---|---|
| Patient provides complete emergency contact | Accept |
| Patient provides contact name but no phone | Show blocking validation if hasEmergencyContact is true |
| Patient cannot provide emergency contact | Accept with emergencyContactIsUnavailable=true and reason |
| Patient refuses to provide emergency contact | Accept with reason |
| Migrant/vulnerable patient has no contact | Accept with reason |
| Emergency contact fields missing and hasEmergencyContact=false | Accept |

---

## 5. Insurance/social security behavior

| Scenario | Expected behavior |
|---|---|
| Patient has IMSS | Accept hasSocialSecurity=true and socialSecurityProvider=IMSS |
| Patient has ISSSTE | Accept hasSocialSecurity=true and socialSecurityProvider=ISSSTE |
| Patient has no coverage | Accept hasSocialSecurity=false and hasPrivateInsurance=false |
| Patient does not know coverage | Accept insuranceInformationUnavailable=true and reason |
| Patient has OTHER provider | Require socialSecurityProviderOther |
| Patient has private insurance | Accept privateInsuranceProvider when available |
| Patient refuses to answer | Accept unavailable flag and reason |

---

## 6. Required enum values

socialSecurityProvider values:

- IMSS;
- ISSSTE;
- PEMEX;
- SEDENA;
- SEMAR;
- STATE_PUBLIC_SERVICE;
- PRIVATE;
- NONE;
- UNKNOWN;
- OTHER.

emergencyContactRelationship values:

- SPOUSE;
- PARENT;
- CHILD;
- SIBLING;
- RELATIVE;
- FRIEND;
- GUARDIAN;
- OTHER;
- UNKNOWN.

unavailable reason examples:

- DOES_NOT_KNOW;
- REFUSED_TO_ANSWER;
- NO_CONTACT_AVAILABLE;
- NO_DOCUMENTS_AVAILABLE;
- MIGRANT_OR_TRANSIENT;
- EMERGENCY_OR_FAST_INTAKE;
- OTHER.

---

## 7. Validation rules

| Rule | Severity | Frontend behavior | Backend behavior |
|---|---|---|---|
| hasEmergencyContact missing | Blocking | Show error | Reject |
| hasEmergencyContact=true without emergencyContactFullName | Blocking | Show error | Reject |
| hasEmergencyContact=true without emergencyContactPhoneNumber | Blocking | Show error | Reject |
| hasEmergencyContact=true without emergencyContactRelationship | Blocking | Show error | Reject |
| emergencyContactIsUnavailable=true without emergencyContactUnavailableReason | Blocking | Show error | Reject |
| hasSocialSecurity missing | Blocking | Show error | Reject |
| hasPrivateInsurance missing | Blocking | Show error | Reject |
| socialSecurityProvider=OTHER without socialSecurityProviderOther | Blocking | Show error | Reject |
| insuranceInformationUnavailable=true without insuranceInformationUnavailableReason | Blocking | Show error | Reject |
| unusual phone format | Warning | Allow save with warning | Accept normalized/original |
| optional notes missing | Non-blocking | Allow save | Accept |

---

## 8. Offline sync contract

Patient intake sync payload must include these fields when available:

| Field | Required | Purpose |
|---|---:|---|
| hasEmergencyContact | Yes | Contact section state |
| emergencyContactFullName | Conditional | Emergency contact identity |
| emergencyContactPhoneNumber | Conditional | Emergency contact phone |
| emergencyContactRelationship | Conditional | Emergency contact relationship |
| emergencyContactIsUnavailable | Yes | Explicit unavailable marker |
| emergencyContactUnavailableReason | Conditional | Reason for unavailable contact |
| hasSocialSecurity | Yes | Coverage state |
| socialSecurityProvider | Conditional | Public provider |
| socialSecurityProviderOther | Conditional | Other provider details |
| hasPrivateInsurance | Yes | Private coverage state |
| privateInsuranceProvider | Conditional | Private provider |
| insuranceInformationUnavailable | Yes | Explicit unavailable marker |
| insuranceInformationUnavailableReason | Conditional | Reason for unavailable coverage |

Required rejection examples:

- emergency_contact_name_missing;
- emergency_contact_phone_missing;
- emergency_contact_relationship_missing;
- emergency_contact_unavailable_reason_missing;
- social_security_provider_other_missing;
- insurance_unavailable_reason_missing.

---

## 9. Spanish frontend labels

| Field | Label |
|---|---|
| hasEmergencyContact | Tiene contacto de emergencia |
| emergencyContactFullName | Nombre del contacto de emergencia |
| emergencyContactPhoneNumber | Telefono del contacto de emergencia |
| emergencyContactRelationship | Relacion con el paciente |
| emergencyContactNotes | Notas del contacto de emergencia |
| emergencyContactIsUnavailable | No tiene contacto de emergencia disponible |
| emergencyContactUnavailableReason | Motivo por el que no hay contacto |
| hasSocialSecurity | Tiene seguro social |
| socialSecurityProvider | Institucion de seguro social |
| socialSecurityProviderOther | Otra institucion |
| hasPrivateInsurance | Tiene seguro privado |
| privateInsuranceProvider | Aseguradora privada |
| insuranceCoverageNotes | Notas de cobertura |
| insuranceInformationUnavailable | No conoce o no proporciona informacion de seguro |
| insuranceInformationUnavailableReason | Motivo por el que no hay informacion de seguro |

---

## 10. Privacy and logging requirements

Never log:

- emergencyContactFullName;
- emergencyContactPhoneNumber;
- emergencyContactRelationship;
- emergencyContactNotes;
- socialSecurityProvider;
- socialSecurityProviderOther;
- privateInsuranceProvider;
- insuranceCoverageNotes;
- future social security identifiers;
- future policy numbers;
- PayloadJson.

Allowed telemetry:

- correlation id;
- request id;
- sanitized endpoint route;
- status code;
- elapsed time;
- non-sensitive validation outcome.

---

## 11. Frontend readiness result

Frontend readiness after this phase:

PARTIAL_PATIENT_DETAILS_FRONTEND_READY

Allowed after P3-30C:

- emergency contact UI mock;
- insurance/social security UI mock;
- frontend validation copy;
- mocked save behavior;
- offline pending state mock.

Not allowed until P3-30D:

- final API integration;
- final generated client;
- final OpenAPI-based implementation;
- production API integration.
---

## 12. P3-30D OpenAPI/frontend contract freeze

P3-30D freezes the frontend API contract after emergency contact and insurance/social security contracts are complete.

After P3-30D, frontend patient details scaffolding is allowed in mock API mode.
