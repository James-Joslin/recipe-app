# Recipe Capture and Meal Planning App — Delivery Backlog

## Product synopsis

Build a recipe capture and meal-planning application that lets users create recipes manually or import them from URLs, structured web data, XML/RSS/Atom feeds, photographs, and PDFs. Every ingestion route must produce the same versioned `RecipeDraft` contract, preserve its source evidence, expose uncertainty, and require user review where necessary before publishing a canonical recipe.

Users remain in control of day and week planning. The application can suggest complementary meals based on household preferences, dietary constraints, variety, effort, ingredient reuse, and leftovers, but it must never alter a plan without an explicit user action. PostgreSQL with JSONB is the authoritative store; pgvector provides initial semantic retrieval. Neo4j is a later, evidence-based option for valuable multi-hop relationships, not a prerequisite for similarity search or the MVP.

## Backlog conventions

- Tasks are listed in intended implementation order. A later task may start early only when all of its stated dependencies and interface contracts are satisfied.
- IDs identify the primary workstream: `PRD` product, `ARC` architecture, `UX` design, `FE` web frontend, `MOB` mobile, `DB` database, `API` application API, `ING` ingestion, `ML` model/OCR inference, `REC` recommendations, `SEC` security/privacy, `QA` quality, `DEV` developer platform, and `OPS` operations.
- “Canonical” means reviewed application data in PostgreSQL. Raw captures, OCR results, model outputs, embeddings, and graph projections are derived or evidential data and must not silently overwrite canonical records.
- The task-level Definition of Done supplements the baseline Definition of Done below.

## Baseline Definition of Done

Every implementation task is complete only when:

- The agreed requirements and acceptance behaviour are implemented and reviewed.
- Automated tests cover normal, boundary, permission, failure, and retry cases appropriate to the change.
- API/schema changes are versioned, documented, and compatible with affected clients and workers.
- Logging, metrics, error handling, privacy, accessibility, and security requirements are addressed.
- Database changes include forward migration, safe rollout notes, and tested rollback or remediation steps.
- User-facing behaviour has design/product acceptance and no unresolved critical or high-severity defects.
- Operationally significant changes include dashboards, alerts, runbooks, and ownership where applicable.
- Documentation and release notes are updated, and the change is deployable through the normal pipeline.

## Delivery sequence

| Phase | Outcome |
| --- | --- |
| 0. Definition | Signed-off scope, domain vocabulary, journeys, risks, and measurable quality corpus. |
| 1. Foundations | Secure, observable, deployable application and worker skeletons. |
| 2. Canonical platform | Versioned data model and APIs shared by every ingestion route. |
| 3. Manual recipe library | Users can create, edit, organise, search, export, and delete recipes. |
| 4. URL and XML ingestion | Safe, evidence-backed web/feed imports create reviewable drafts. |
| 5. Photo and PDF ingestion | OCR-first extraction produces evidence-linked, correctable drafts. |
| 6. Planning and shopping | Users manually plan meals and derive editable shopping lists. |
| 7. Contextual suggestions | Optional, explainable meal suggestions augment manual planning. |
| 8. Beta, launch, and improvement | The product is hardened, released safely, measured, and iterated. |

---

# Phase 0 — Product and technical definition

## PRD-001 — Define the MVP, users, and measurable outcomes

**Workstream:** Product  
**Depends on:** None

**Requirements**

- Identify primary personas, household model, supported devices, launch markets, languages, and accessibility expectations.
- Define MVP boundaries for manual entry, URL/XML import, photo/PDF capture, review, planning, shopping, and suggestions.

**Implementation**

- Produce an approved product brief containing in-scope/out-of-scope capability, assumptions, constraints, risks, and measurable outcomes.
- Record open decisions with owners and due dates; avoid committing to extraction accuracy before benchmark data exists.

**Definition of Done**

- Product, design, engineering, privacy, and operations approve one versioned MVP brief.
- Every Phase 0–8 epic maps to an explicit product outcome and has an accountable owner.

## UX-001 — Map end-to-end user journeys and failure recovery

**Workstream:** UX / Product  
**Depends on:** PRD-001

**Requirements**

- Cover manual creation, URL/XML import, photo/PDF capture, correction, publish, search, planning, shopping, suggestions, export, and deletion.
- Define recoverable states for invalid URLs, unsupported pages, poor images, OCR uncertainty, provider failure, and offline interruption.

**Implementation**

- Create journey maps and low-fidelity flows for web and mobile, including permission, progress, empty, error, and retry states.
- Identify consequential fields such as quantity, unit, allergen, time, and temperature that need stronger review treatment.

**Definition of Done**

- Each journey has an entry point, success state, cancellation path, recoverable failure path, and acceptance criteria.
- Product and engineering confirm that the flows can be represented by the planned domain state machines.

## ARC-001 — Approve architecture decisions and service boundaries

**Workstream:** Architecture  
**Depends on:** PRD-001, UX-001

**Requirements**

- Establish PostgreSQL/JSONB as the system of record, object storage for media, Redis-backed jobs, pgvector for initial embeddings, and Neo4j as optional.
- Separate synchronous business APIs from asynchronous ingestion/model workers while retaining one versioned contract.

**Implementation**

- Record Architecture Decision Records for frontend/mobile frameworks, API and worker stacks, storage, queues, authentication, observability, deployment, and model-provider abstraction.
- Define ownership for canonical data, raw evidence, derived vectors, caches, and any future graph projection.

**Definition of Done**

- ADRs are reviewed, version-controlled, and referenced by downstream tasks.
- No component has ambiguous write authority over canonical recipe or meal-plan data.

## SEC-001 — Define privacy position and initial threat model

**Workstream:** Security / Privacy  
**Depends on:** PRD-001, ARC-001

**Requirements**

- Address private household recipes, uploaded images/PDFs, source URLs, user feedback, OCR/model providers, analytics, export, retention, and deletion.
- Model threats including tenant leakage, SSRF, malicious uploads, insecure signed URLs, prompt injection, secret exposure, and excessive model/provider retention.

**Implementation**

- Produce a data-flow diagram, classification scheme, processing inventory, retention matrix, and threat register with mitigations and owners.
- Define consent and configuration rules for cloud OCR, model use, personalisation, telemetry, and feedback reuse.

**Definition of Done**

- Privacy and security approve the MVP data flows and mandatory controls.
- Every high-risk threat has a prevention/detection control and a verification task in the backlog.

## QA-001 — Build the representative extraction benchmark corpus

**Workstream:** QA / Data quality  
**Depends on:** PRD-001, SEC-001

**Requirements**

- Cover clean print, glare, skew, perspective, low light, cut-off text, columns, handwriting, screenshots, multi-page PDFs, JSON-LD, Microdata/RDFa, plain HTML, RSS/Atom, and XML.
- Include ground-truth recipe fields, evidence regions, expected unresolved values, and required attribution.

**Implementation**

- Create consented/versioned fixtures with source-class labels and a documented sampling policy.
- Define metrics for OCR error, field precision/recall, evidence coverage, unsupported inference, correction time, completion, and abandonment.

**Definition of Done**

- The corpus is accessible to approved test environments, contains no unapproved personal data, and has a versioned manifest.
- A baseline evaluation can run repeatedly and report results by content class rather than one aggregate score.

---

# Phase 1 — Engineering foundations

## DEV-001 — Create repositories and shared project structure

**Workstream:** Developer platform  
**Depends on:** ARC-001

**Requirements**

- Support a Next.js/TypeScript web app, React Native/Expo mobile app, NestJS/TypeScript API, and Python/FastAPI worker services.
- Share generated API types, schemas, lint rules, test utilities, and local development conventions without coupling release cycles unnecessarily.

**Implementation**

- Scaffold projects, package management, workspace boundaries, code ownership, formatting, linting, commit checks, and contribution documentation.
- Add reproducible local containers for dependencies and sample environment files containing no real secrets.

**Definition of Done**

- A new engineer can install, build, test, and run all skeleton services from documented steps.
- Protected branches and code-owner reviews are active for security- and schema-sensitive areas.

## DEV-002 — Provision development and staging infrastructure

**Workstream:** DevOps / Cloud  
**Depends on:** DEV-001, SEC-001

**Requirements**

- Provide isolated PostgreSQL, object storage, Redis/queue, secret management, application hosting, worker hosting, and network boundaries.
- Keep development, staging, and production credentials and data fully separated.

**Implementation**

- Define infrastructure as code, environment configuration, private networking, encryption, storage lifecycle rules, and least-privilege service identities.
- Configure private media buckets and short-lived signed upload/download URLs.

**Definition of Done**

- Development and staging can be recreated from code and pass connectivity/security smoke tests.
- No service requires long-lived credentials in source code, images, or client applications.

## DB-001 — Bootstrap PostgreSQL, extensions, and migration discipline

**Workstream:** Database  
**Depends on:** DEV-002, ARC-001

**Requirements**

- Enable required PostgreSQL features, JSONB, full-text/trigram support, and pgvector in all environments.
- Establish naming, ownership, indexing, migration, seed, backup, and test-database standards.

**Implementation**

- Create the initial database, roles, extension migrations, schema namespaces, migration runner, seed framework, and local reset workflow.
- Add automated migration validation against empty and representative prior-version databases.

**Definition of Done**

- CI can create and migrate a clean database and detect irreversible or unsafe migration patterns.
- Application and worker roles have only the permissions defined by the architecture and threat model.

## API-001 — Establish the API skeleton and contract workflow

**Workstream:** API  
**Depends on:** DEV-001, DEV-002, DB-001

**Requirements**

- Provide health/readiness endpoints, request IDs, consistent errors, validation, versioned routes, and generated client contracts.
- Support idempotency and optimistic concurrency for future mutation endpoints.

**Implementation**

- Configure NestJS modules, OpenAPI generation, Zod/JSON Schema boundaries, database access, error mapping, rate limits, and correlation IDs.
- Generate TypeScript client types and establish schema compatibility checks for Python workers.

**Definition of Done**

- Web, mobile, and Python test clients can call the deployed staging skeleton using generated contracts.
- Invalid inputs produce stable, documented errors without leaking stack traces or sensitive data.

## API-002 — Establish asynchronous job and worker foundations

**Workstream:** API / Workers  
**Depends on:** API-001, DEV-002

**Requirements**

- Support durable, idempotent ingestion jobs with retries, timeouts, cancellation, progress, and dead-letter handling.
- Ensure large media and OCR payloads stay in object storage rather than queue messages or hot relational rows.

**Implementation**

- Define queue names, payload envelopes, job identity, state-transition rules, retry policies, leases, worker heartbeats, and dead-letter replay tooling.
- Add a Python worker skeleton with structured validation against the common job and `RecipeDraft` contracts.

**Definition of Done**

- A sample job can be submitted, processed, retried safely, cancelled, observed, and replayed without duplicate domain records.
- Queue and worker failure states are visible in staging dashboards.

## SEC-002 — Implement authentication and household isolation skeleton

**Workstream:** Security / API  
**Depends on:** API-001, DEV-002

**Requirements**

- Use managed OIDC authentication and represent household membership, roles, invitations, and record ownership.
- Reject cross-household access at both API policy and database-policy layers where practical.

**Implementation**

- Integrate token validation, session handling, household context, role guards, audit identity, and initial row-level-security policies.
- Add automated tests for missing, expired, forged, wrong-audience, and cross-household credentials.

**Definition of Done**

- Authenticated users can access only their authorised household context in web, mobile, API, and worker flows.
- Security tests demonstrate denial of horizontal and vertical privilege escalation.

## OPS-001 — Implement CI/CD, observability, and operational baselines

**Workstream:** DevOps / Operations  
**Depends on:** DEV-001, DEV-002, API-001, API-002

**Requirements**

- Automate linting, unit/contract tests, dependency and image scanning, builds, migrations, deployments, and rollback.
- Collect redacted logs, metrics, traces, errors, queue health, database health, and deployment markers.

**Implementation**

- Configure pipelines, signed artefacts where supported, environment approvals, OpenTelemetry, dashboards, alert routing, and service ownership.
- Add content-redaction rules so raw recipe text, source images, tokens, and signed URLs are excluded from general telemetry.

**Definition of Done**

- A reviewed change deploys automatically to staging and can be promoted and rolled back using documented procedures.
- A synthetic request can be traced across client, API, queue, worker, database, and object storage without exposing private content.

---

# Phase 2 — Canonical domain, database, and APIs

## DB-002 — Implement the versioned canonical recipe schema

**Workstream:** Database  
**Depends on:** DB-001, QA-001

**Requirements**

- Represent recipes, immutable recipe versions, ingredient sections/lines, steps, servings, durations, tags, attribution, ownership, and publish state.
- Preserve both display text and parsed values; a later recipe edit must not silently rewrite historical meal plans.

**Implementation**

- Create relational tables for identity and frequently queried fields, plus validated JSONB for extensible structured content.
- Add constraints, ordering keys, version lineage, content hashes, soft-draft handling, and indexes for list/detail access.

**Definition of Done**

- Migrations and schema tests cover create, revise, publish, copy, restore, and concurrent-edit cases.
- Representative recipes round-trip without losing original ingredient or instruction text.

## DB-003 — Implement sources, extraction runs, and field evidence

**Workstream:** Database  
**Depends on:** DB-002, API-002

**Requirements**

- Store source assets, canonical URLs, fetch metadata, ingestion jobs, extraction runs, parser/model versions, raw output references, field candidates, confidence, warnings, and evidence pointers.
- Permit multiple extraction attempts without overwriting prior evidence or the user's canonical recipe.

**Implementation**

- Create source, asset, extraction, field-evidence, warning, and provenance tables linked to object-storage keys and versioned drafts.
- Define evidence coordinates for page/block/line/bounding-box and source fragments for web/XML imports.

**Definition of Done**

- Every candidate field can be traced to a source span/region and extraction version.
- Reprocessing creates a new run and comparison data while preserving previous runs and user corrections.

## DB-004 — Implement ingestion and review state machines

**Workstream:** Database / API  
**Depends on:** DB-003

**Requirements**

- Support queued, fetching/uploaded, preprocessing, extracting, validating, ready, needs-review, failed, cancelled, and published outcomes.
- Make retries idempotent and prohibit invalid or regressive state transitions.

**Implementation**

- Define transition guards, idempotency keys, attempt records, error classification, progress events, cancellation semantics, and expiry rules.
- Expose a transactional transition service used by APIs and workers rather than allowing ad hoc state writes.

**Definition of Done**

- State-transition tests cover success, duplicate delivery, timeout, provider outage, cancellation, retry, dead-letter replay, and publish.
- No retry can create duplicate recipes, assets, or extraction runs for the same idempotency scope.

## DB-005 — Implement canonical foods, units, aliases, and conversions

**Workstream:** Database / Data  
**Depends on:** DB-002, PRD-001

**Requirements**

- Represent canonical foods, locale-aware aliases, units, dimensions, conservative conversions, preparation notes, optional quantities, ranges, and raw text.
- Never force an unsafe conversion or discard unresolved wording.

**Implementation**

- Seed the initial vocabulary and unit registry; add alias provenance, review status, conversion precision, and ambiguity markers.
- Implement deterministic parsing/normalisation interfaces that can evolve independently from canonical display text.

**Definition of Done**

- Agreed fixture lines parse and round-trip with expected quantities, units, foods, modifiers, and unresolved states.
- Conversion tests reject incompatible dimensions and preserve uncertainty and original text.

## DB-006 — Implement library, preference, planning, and feedback tables

**Workstream:** Database  
**Depends on:** DB-002, DB-005, SEC-002

**Requirements**

- Model collections, favourites, ratings, household preferences, dietary exclusions, meal plans, day/slot entries, shopping lists/items, suggestion impressions, acceptances, and dismissals.
- Meal-plan entries must reference deliberate recipe versions or snapshots.

**Implementation**

- Add ownership, ordering, timestamps, audit fields, uniqueness constraints, and indexes for calendar and recommendation queries.
- Separate hard restrictions from soft preferences and user-entered shopping items from generated items.

**Definition of Done**

- Schema tests demonstrate deterministic add/move/copy/remove plan behaviour and editable shopping-list lineage.
- Feedback records can be measured without storing unnecessary private recipe content in analytics systems.

## DB-007 — Implement the transactional outbox and derived-data contracts

**Workstream:** Database / Architecture  
**Depends on:** DB-002, DB-003, DB-006

**Requirements**

- Reliably emit changes needed for search documents, embeddings, caches, analytics, and a possible future graph projection.
- Derived consumers must be idempotent and rebuildable from PostgreSQL.

**Implementation**

- Create an outbox table, publisher, event envelope, sequence/version rules, consumer checkpoints, replay tooling, and tombstones for deletion.
- Define stable domain IDs and projection versioning without adding Neo4j as a write dependency.

**Definition of Done**

- Transaction tests prove domain data and its event commit atomically.
- Search/vector projections can be rebuilt and deleted without modifying canonical records.

## API-003 — Implement recipe draft, publish, and version APIs

**Workstream:** API  
**Depends on:** DB-002, DB-004, SEC-002

**Requirements**

- Support create, read, autosave, validate, publish, revise, copy, restore, and delete operations with household permissions.
- Require optimistic concurrency and stable validation errors suitable for field-level frontend display.

**Implementation**

- Build versioned endpoints/services, ETags or revision tokens, idempotency support, transaction boundaries, audit events, and generated client types.
- Validate the same `RecipeDraft`/canonical schemas used by workers; keep unresolved fields explicit.

**Definition of Done**

- Contract and integration tests cover the complete draft-to-published lifecycle, conflicts, permissions, and invalid data.
- Web and mobile clients can consume the generated contract without hand-written duplicate models.

## API-004 — Implement library search, collections, and preferences APIs

**Workstream:** API / Search  
**Depends on:** DB-005, DB-006, API-003, DB-007

**Requirements**

- Support paginated recipe browsing, full-text/trigram search, filters, collections, favourites, ratings, and household preference management.
- Return stable ordering, explainable filter behaviour, and permission-safe results.

**Implementation**

- Build query services and indexes for title, ingredient, tag, cuisine, source, time, and user metadata.
- Add endpoints for hard dietary rules and soft likes/dislikes with appropriate validation and audit history.

**Definition of Done**

- Search relevance and performance meet agreed fixture and load-test thresholds.
- Cross-household records never appear in results, counts, facets, or error details.

## QA-002 — Establish domain contract and database regression suites

**Workstream:** QA / Automation  
**Depends on:** DB-002 through DB-007, API-003, API-004

**Requirements**

- Prove that TypeScript clients/APIs and Python workers accept and reject the same schema versions.
- Cover migrations, state machines, tenancy, concurrency, outbox delivery, deletion tombstones, and representative recipe fixtures.

**Implementation**

- Build reusable factories, golden JSON fixtures, property-based unit tests, database integration tests, and backward-compatibility checks.
- Gate schema changes on explicit versioning and fixture updates.

**Definition of Done**

- The suites run in CI with deterministic results and actionable failure output.
- An intentionally incompatible contract or invalid state transition is blocked before deployment.

---

# Phase 3 — Manual recipe library

## FE-001 — Build the responsive application shell and design system

**Workstream:** Web frontend / UX  
**Depends on:** UX-001, API-001, SEC-002

**Requirements**

- Provide authenticated navigation for library, imports, planner, shopping, settings, and household context.
- Meet keyboard, focus, contrast, responsive, loading, empty, error, and notification requirements.

**Implementation**

- Create reusable tokens/components, route structure, authenticated layouts, data-fetching conventions, form patterns, error boundaries, and analytics hooks.
- Add Storybook or equivalent component documentation and automated accessibility checks.

**Definition of Done**

- Core components pass design review, keyboard checks, automated accessibility checks, and responsive viewport testing.
- The deployed shell handles sign-in, sign-out, household switching, expired sessions, and service errors.

## FE-002 — Build recipe library list, search, and detail views

**Workstream:** Web frontend  
**Depends on:** FE-001, API-004

**Requirements**

- Let users browse, search, filter, sort, paginate, inspect, favourite, rate, and add recipes to collections.
- Clearly show source attribution, version state, unresolved warnings, and private household ownership.

**Implementation**

- Connect query/filter state to URLs, implement accessible cards/tables, optimistic low-risk actions, cache invalidation, and empty/no-result states.
- Build a recipe detail view with ingredients, instructions, metadata, provenance, and edit/copy actions.

**Definition of Done**

- All library behaviours work with generated API types and survive refresh/deep links.
- Search/filter acceptance fixtures and keyboard/screen-reader journeys pass.

## FE-003 — Build the structured manual recipe editor

**Workstream:** Web frontend  
**Depends on:** FE-001, API-003, DB-005

**Requirements**

- Edit title, description, servings, times, tags, source notes, ingredient sections/lines, instruction steps, and optional metadata.
- Autosave drafts without interrupting work and show validation only at useful moments, especially before publish.

**Implementation**

- Implement typed forms, local dirty state, debounced autosave, concurrency-conflict handling, reorder controls, undo-friendly interactions, and accessible errors.
- Preserve raw ingredient/instruction text while showing parsed fields and unresolved warnings.

**Definition of Done**

- A user can create and publish representative recipes using keyboard, pointer, and mobile-width layouts.
- Autosave, offline interruption, server validation, and concurrent-edit conflicts have tested recovery paths.

## FE-004 — Add paste helpers, version history, copy, and restore

**Workstream:** Web frontend / API  
**Depends on:** FE-003, API-003

**Requirements**

- Convert pasted ingredient or instruction blocks into editable lines without discarding the original paste.
- Allow users to view differences, copy a recipe, create revisions, and restore/copy a prior version safely.

**Implementation**

- Add deterministic line splitting, preview/confirm, version timeline, field-level diff display, and restore-as-new-version behaviour.
- Record source and user attribution for copied/restored versions.

**Definition of Done**

- Golden paste fixtures produce expected editable structures and always permit manual correction.
- Version operations preserve history and never mutate historical meal-plan references.

## API-005 — Implement household export and deletion workflows

**Workstream:** API / Privacy  
**Depends on:** DB-007, API-003, API-004, SEC-002

**Requirements**

- Export user/household recipes, plans, lists, preferences, provenance, and relevant media metadata in a documented portable format.
- Delete canonical and derived data according to ownership, retention, legal, and recovery rules.

**Implementation**

- Build authenticated export jobs, signed result delivery, deletion confirmation, tombstone/outbox events, delayed object removal, and status tracking.
- Propagate deletion to search, vectors, caches, analytics identifiers, and future graph projections.

**Definition of Done**

- Privacy tests confirm complete authorised export and end-to-end deletion propagation.
- Failures are retryable and visible; partial deletion cannot be reported as complete.

## QA-003 — Certify the manual library release slice

**Workstream:** QA  
**Depends on:** FE-002, FE-003, FE-004, API-005

**Requirements**

- Cover create/edit/publish, paste, search, organise, favourite, rate, copy, restore, export, delete, tenancy, concurrency, and accessibility.
- Validate supported browsers and agreed mobile web breakpoints.

**Implementation**

- Add end-to-end suites, visual regression for critical screens, exploratory charters, and seeded test households.
- Measure draft-save reliability, search latency, and user journey completion in staging.

**Definition of Done**

- All manual-library acceptance journeys pass with no unresolved critical/high defects.
- The slice is independently releasable behind a feature flag and has support notes.

---

# Phase 4 — URL, HTML, and XML ingestion

## ING-001 — Build the hardened URL fetch service

**Workstream:** Ingestion / Security  
**Depends on:** API-002, DB-003, DB-004, SEC-001

**Requirements**

- Validate schemes, hostnames, DNS results, redirects, content type, response size, timeouts, and rate limits.
- Block private, loopback, link-local, metadata-service, and disallowed network destinations throughout redirects and DNS changes.

**Implementation**

- Create an isolated fetcher with egress controls, safe DNS/IP checks, redirect limits, streaming size enforcement, content hashing, canonical URL capture, and audit metadata.
- Store raw responses privately according to retention policy and return typed recoverable errors.

**Definition of Done**

- SSRF and resource-exhaustion tests pass, including redirect, DNS rebinding, compression bomb, slow response, and oversized payload cases.
- Approved public sources can be fetched repeatably without exposing internal networks or secrets.

## ING-002 — Parse structured recipe data from web pages

**Workstream:** Ingestion  
**Depends on:** ING-001, DB-005

**Requirements**

- Prefer schema.org `Recipe` JSON-LD, then Microdata/RDFa, and handle pages containing multiple candidate recipes.
- Preserve canonical URL, author/publisher, source fragments, raw values, parser version, and warnings.

**Implementation**

- Build standards-aware parsers and map outputs through a shared adapter into `RecipeDraft` with evidence pointers.
- Implement deterministic handling for ISO durations, ingredient arrays, instructions, nested sections, images, and duplicates.

**Definition of Done**

- Reference-site fixtures produce the expected draft fields, attribution, warnings, and evidence.
- Parser failures remain recoverable and never create a misleading published recipe.

## ING-003 — Implement XML, RSS, and Atom adapters

**Workstream:** Ingestion  
**Depends on:** ING-001, ING-002

**Requirements**

- Detect supported feed/document types and extract recipe links or recipe content from RSS, Atom, and selected XML schemas.
- Parse XML securely with external entities and dangerous expansion disabled.

**Implementation**

- Create a format detector, safe XML parser, adapter registry, namespace handling, pagination/feed metadata, and source-evidence mapping.
- Route discovered recipe URLs through the same canonical fetch/parser pipeline with deduplication.

**Definition of Done**

- Approved XML/feed fixtures import deterministically and malicious XML fixtures are rejected safely.
- Duplicate feed items and repeated imports do not create duplicate canonical recipes.

## ING-004 — Add deterministic HTML fallback and isolated rendering

**Workstream:** Ingestion  
**Depends on:** ING-002, ING-003

**Requirements**

- Extract likely title, metadata, ingredient, and instruction regions when structured data is absent or incomplete.
- Use browser rendering only for legitimate JavaScript-dependent pages and keep it isolated from private networks and application credentials.

**Implementation**

- Build semantic DOM heuristics, readability/noise removal, heading/list grouping, confidence signals, and an allow-controlled rendering worker.
- Capture the exact source DOM/text fragments used for every field candidate.

**Definition of Done**

- Unstructured benchmark pages create useful reviewable drafts with measured precision/recall.
- Browser jobs obey resource/network limits, are observable, and fail closed on unsafe navigation.

## ML-001 — Add constrained language-model fallback for web extraction

**Workstream:** Model inference / Ingestion  
**Depends on:** ING-004, DB-003, QA-001

**Requirements**

- Use a language model only when deterministic adapters leave unresolved structure or wording.
- Require schema-constrained output, source-span support, explicit unresolved values, and no invented quantities, allergens, times, or temperatures.

**Implementation**

- Create a provider-neutral inference interface, versioned prompts/configuration, structured output validation, prompt-injection isolation, token limits, and cost/latency capture.
- Merge model candidates with deterministic results using provenance and confidence rules rather than silent replacement.

**Definition of Done**

- Model outputs failing schema or evidence validation are rejected or marked unresolved.
- Evaluation demonstrates the fallback improves agreed fields without exceeding unsupported-inference thresholds.

## API-006 — Orchestrate URL/XML imports and expose progress

**Workstream:** API / Ingestion  
**Depends on:** ING-001 through ING-004, ML-001, DB-004

**Requirements**

- Accept URLs, choose the correct adapter path, return an ingestion ID immediately, and expose progress, warnings, retry, cancel, and resulting draft.
- Deduplicate appropriate repeat imports while allowing intentional re-import/reprocessing.

**Implementation**

- Add import endpoints, job orchestration, adapter routing, state/event projection, idempotency, quotas, and server-sent/polled progress support.
- Map all adapter outputs to the same versioned draft validation and review contract.

**Definition of Done**

- End-to-end tests cover JSON-LD, HTML fallback, XML/feed, unsupported content, cancellation, retry, and provider/model failure.
- No import publishes automatically or bypasses ownership and review rules.

## FE-005 — Build the URL/XML import and recovery experience

**Workstream:** Web frontend  
**Depends on:** FE-001, API-006

**Requirements**

- Let users submit a URL, monitor progress, review attribution and warnings, open the draft, retry, or choose manual/photo alternatives.
- Make unsupported, unsafe, duplicate, and partially extracted outcomes understandable and recoverable.

**Implementation**

- Build import form, progress timeline, status polling/subscription, result summary, duplicate choices, error-specific actions, and import history entry.

**Definition of Done**

- All URL/XML acceptance journeys work by keyboard and screen reader and survive refresh/navigation.
- User testing confirms errors describe what happened and offer a viable next action.

---

# Phase 5 — Photo and PDF ingestion, OCR, and review

## MOB-001 — Build camera, photo-picker, and PDF capture flows

**Workstream:** Mobile frontend  
**Depends on:** UX-001, API-002, SEC-002

**Requirements**

- Capture or select one/multiple images and PDFs with least-privilege permissions, previews, page ordering, and cancellation.
- Warn about obvious blur, glare, darkness, rotation, missing edges, and oversized/unsupported files before costly upload.

**Implementation**

- Implement native capture/picker integration, local quality checks, crop/rotate controls, resumable signed uploads, progress, retry, and offline-safe draft state.

**Definition of Done**

- Supported iOS/Android devices can capture, reorder, upload, cancel, and resume representative sources.
- Permission denial, backgrounding, network loss, low storage, and invalid-file cases have tested recovery behaviour.

## FE-006 — Build web image and PDF upload flows

**Workstream:** Web frontend  
**Depends on:** FE-001, API-002, SEC-002

**Requirements**

- Support drag/drop and file selection for images and PDFs with preview, ordering, validation, upload progress, retry, and cancellation.
- Use direct signed uploads without exposing storage credentials or public media URLs.

**Implementation**

- Build accessible file controls, client checks, thumbnail/page preview, multipart/resumable upload integration, and import creation after upload confirmation.

**Definition of Done**

- Supported browsers complete single- and multi-page uploads and recover from interrupted or expired signed URLs.
- Accessibility and malicious-file boundary tests pass.

## ML-002 — Implement image preprocessing and quality assessment

**Workstream:** Model inference / Computer vision  
**Depends on:** API-002, QA-001, MOB-001 or FE-006

**Requirements**

- Detect page edges, crop, correct perspective, rotate/deskew, normalise contrast, and measure blur, glare, darkness, and cut-off risk.
- Preserve the original immutable asset and record every transformation/version.

**Implementation**

- Build OpenCV-based preprocessing workers, quality scores, transformation metadata, thumbnails, and configurable thresholds by source class.
- Allow reprocessing with new settings without replacing original or prior derived assets.

**Definition of Done**

- Benchmark results show measurable OCR improvement or an accurate retry warning for targeted degraded classes.
- All transformations are reproducible, versioned, and traceable to the original asset.

## ML-003 — Implement the OCR provider interface and engines

**Workstream:** Model inference / OCR  
**Depends on:** ML-002, DB-003

**Requirements**

- Support on-device/native OCR where appropriate and a selected cloud document OCR fallback through one provider-neutral contract.
- Retain pages, blocks, lines, words, bounding boxes, reading order, language, and confidence.

**Implementation**

- Define OCR request/result schemas, adapters, provider routing, timeout/retry/circuit-breaker behaviour, quotas, cost capture, and privacy configuration.
- Store large OCR artefacts in private object storage with indexed metadata in PostgreSQL.

**Definition of Done**

- Both configured OCR paths pass contract fixtures and can be swapped without changing downstream parsers.
- Provider outage, throttling, malformed response, and privacy-retention settings are tested and observable.

## ML-004 — Segment recipe regions and reconstruct reading order

**Workstream:** Model inference / Document understanding  
**Depends on:** ML-003, QA-001

**Requirements**

- Identify likely title, metadata, ingredient sections, instruction sections, notes, and unrelated page noise across one or multiple pages.
- Handle columns, continued sections, headings, bullets, page boundaries, and duplicate headers/footers.

**Implementation**

- Build layout/rule features and, where justified, a versioned layout model; output regions and ordered text spans with confidence and evidence coordinates.

**Definition of Done**

- Reading-order and region-assignment metrics meet agreed thresholds for supported benchmark classes.
- Low-confidence or ambiguous layouts are marked for review rather than flattened deceptively.

## ML-005 — Parse OCR text into structured recipe candidates

**Workstream:** Model inference / Parsing  
**Depends on:** ML-004, DB-005

**Requirements**

- Parse fractions, mixed numbers, ranges, optional quantities, units, ingredient names, modifiers, headings, durations, temperatures, and ordered steps.
- Preserve raw OCR text, evidence spans, confidence, and unresolved values.

**Implementation**

- Implement deterministic locale-aware parsers first, then invoke the constrained model interface only for unresolved structure or difficult wording.
- Add schema/cardinality/plausibility validation and safeguards against invented safety-sensitive values.

**Definition of Done**

- Golden OCR fixtures produce expected `RecipeDraft` candidates and evidence links.
- Unsupported inference and consequential-field error rates remain within agreed benchmark gates.

## API-007 — Orchestrate the photo/PDF extraction pipeline

**Workstream:** API / Model orchestration  
**Depends on:** ML-002 through ML-005, DB-004

**Requirements**

- Coordinate upload completion, preprocessing, OCR, segmentation, parsing, validation, and review readiness across multi-page inputs.
- Support progress, cancellation, re-OCR, page replacement, retries, and provider fallback without duplication.

**Implementation**

- Implement the pipeline DAG/state transitions, per-stage idempotency, progress events, error taxonomy, quota enforcement, and result assembly into `RecipeDraft`.

**Definition of Done**

- End-to-end pipeline tests cover clean, degraded, multi-page, cancelled, retried, and provider-failure cases.
- Every published field remains traceable to evidence and an extraction/model/parser version.

## FE-007 — Build the evidence-linked review and correction workspace

**Workstream:** Web and mobile frontend  
**Depends on:** API-007, FE-003, MOB-001

**Requirements**

- Present the source and structured draft together; selecting a field highlights its supporting source region or fragment.
- Prioritise low-confidence and consequential warnings while allowing correction, unresolved marking, re-OCR, page replacement, and manual completion.

**Implementation**

- Build desktop split view, mobile evidence cards, field-warning navigation, zoom/pan, bounding-box overlays, save/retry controls, and publish validation.
- Ensure suggestions from OCR/model are visually distinguishable from user-confirmed values.

**Definition of Done**

- Users can resolve every supported import without losing source evidence or prior corrections.
- Keyboard, screen-reader, touch-target, zoom, and mobile usability acceptance tests pass.

## QA-004 — Benchmark and certify photo/PDF extraction

**Workstream:** QA / Model evaluation  
**Depends on:** ML-002 through ML-005, API-007, FE-007

**Requirements**

- Report preprocessing, OCR, layout, field extraction, evidence, unsupported inference, retry rate, completion, and correction-time metrics by content class.
- Compare on-device and cloud paths, including cost and latency.

**Implementation**

- Automate corpus evaluation, threshold gates, regression comparison, error slicing, and protected result reporting.
- Add end-to-end usability tests for correction and recovery, not merely OCR character accuracy.

**Definition of Done**

- Agreed MVP source classes pass their class-specific quality gates with no safety-sensitive silent failures.
- Model/parser/provider changes cannot deploy if they cause an unapproved material regression.

## OPS-002 — Add inference cost, quota, and provider operations

**Workstream:** Operations / Model platform  
**Depends on:** ML-003, API-007, OPS-001

**Requirements**

- Track processing volume, latency, retry, fallback, error, and cost per successful import without logging private content.
- Protect the service from runaway jobs, provider quota exhaustion, and cascading outages.

**Implementation**

- Add budgets, per-household/service limits, circuit breakers, concurrency controls, dashboards, alerts, dead-letter review, and provider outage runbooks.

**Definition of Done**

- Operators can identify a failing stage/provider, contain spend, replay safe failures, and communicate user-visible impact.
- Load and outage exercises demonstrate graceful degradation and recoverability.

---

# Phase 6 — Manual planning and shopping

## API-008 — Implement manual meal-plan operations

**Workstream:** API  
**Depends on:** DB-006, API-003, SEC-002

**Requirements**

- Create/read/update plans and add, move, copy, replace, and remove recipe-version references in day/meal slots.
- Preserve manual control and deterministic results; no recommendation process may mutate a plan.

**Implementation**

- Build transactional endpoints, concurrency controls, timezone/date handling, audit events, and conflict responses for web/mobile sync.

**Definition of Done**

- Contract tests prove deterministic calendar behaviour, household isolation, concurrent edits, and historical recipe-version stability.
- Every plan mutation originates from an explicit authorised user action.

## FE-008 — Build the responsive day/week meal planner

**Workstream:** Web frontend  
**Depends on:** FE-002, API-008

**Requirements**

- Display day/week views and let users find recipes and add, move, copy, replace, or remove meals.
- Work with pointer, keyboard, touch, and non-drag alternatives.

**Implementation**

- Build calendar/agenda views, recipe picker, optimistic updates with rollback, accessible move controls, conflict messaging, and useful empty states.

**Definition of Done**

- Core planning journeys pass responsive, keyboard, screen-reader, concurrency, and refresh persistence tests.
- No suggestion is visually represented as planned until the user confirms it.

## MOB-002 — Build the native mobile planner experience

**Workstream:** Mobile frontend  
**Depends on:** MOB-001, API-008

**Requirements**

- Provide touch-friendly day/week planning, recipe search/picker, add/move/copy/remove actions, and resilient network behaviour.
- Maintain local pending state safely when backgrounded or temporarily offline.

**Implementation**

- Build native screens, local cache, mutation queue, conflict prompts, deep links, and accessibility semantics.

**Definition of Done**

- Supported devices pass planning journeys under normal, slow, offline, reconnect, and concurrent-edit scenarios.
- Pending operations never duplicate or silently overwrite server plan entries.

## API-009 — Generate and maintain editable shopping lists

**Workstream:** API / Database  
**Depends on:** API-008, DB-005, DB-006

**Requirements**

- Aggregate planned recipe ingredients with conservative merge/conversion rules and retain source-plan/recipe lineage.
- Let users split, merge, edit, check, uncheck, add, and remove items without losing manual changes on regeneration.

**Implementation**

- Create generation/version logic, compatible-unit aggregation, ambiguity handling, manual-item flags, reconciliation rules, and transactional endpoints.

**Definition of Done**

- Fixture plans produce expected items, quantities, unresolved groups, and lineage.
- Regeneration preserves or explicitly reconciles user edits and checked states.

## FE-009 — Build shopping list and mobile store mode

**Workstream:** Web and mobile frontend  
**Depends on:** API-009, FE-008, MOB-002

**Requirements**

- Display generated/manual items and support check, edit, add, delete, split, merge, and provenance inspection.
- Provide a mobile store mode with large targets, reliable offline interaction, and conflict handling.

**Implementation**

- Build grouped list views, optimistic check-off, edit/reconciliation prompts, local mutation queue, sync status, and accessible controls.

**Definition of Done**

- Shopping journeys pass online/offline/reconnect, accessibility, concurrent-edit, and regeneration tests.
- Users can always distinguish and control generated versus manually added content.

---

# Phase 7 — Contextual meal suggestions

## ML-006 — Define and generate versioned recipe embeddings

**Workstream:** Model inference / Recommendations  
**Depends on:** DB-007, DB-005, QA-004

**Requirements**

- Generate embeddings only from reviewed recipe versions using a documented projection of title, ingredients, cuisine, method, tags, and other approved fields.
- Exclude raw OCR noise and version the model, projection, dimensions, and creation time.

**Implementation**

- Build the embedding worker, batching, retries, tombstone handling, content hashing, re-embedding workflow, and privacy/cost controls.

**Definition of Done**

- Reviewed recipe changes create/update the correct vector and deletion removes it through the outbox flow.
- Offline checks demonstrate sensible semantic neighbours and reproducible version metadata.

## DB-008 — Implement pgvector indexing and recommendation feature views

**Workstream:** Database / Search  
**Depends on:** DB-006, ML-006

**Requirements**

- Support efficient cosine-distance retrieval plus relational filtering by household access, diet, availability, meal type, history, and recipe status.
- Expose features for ingredient overlap, leftovers, variety, effort, recency, ratings, and feedback.

**Implementation**

- Add vector tables/indexes, query tuning, feature/materialized views where justified, freshness metadata, and rebuild tooling.

**Definition of Done**

- Retrieval meets agreed relevance/latency targets at representative scale and never bypasses hard filters.
- Index rebuild and model-version migration procedures are tested.

## REC-001 — Implement eligibility filters and candidate retrieval

**Workstream:** Recommendation service  
**Depends on:** DB-008, API-008

**Requirements**

- Build context from the anchor meal, selected day/week, preferences, exclusions, recent history, favourites, pantry/shopping overlap, and leftovers.
- Apply allergies/dietary rules and explicit exclusions before ranking.

**Implementation**

- Retrieve candidates from favourites, full-text search, embedding neighbours, ratings/history, and explicit ingredient relationships; merge and deduplicate them.

**Definition of Done**

- Eligibility fixtures show zero known hard-constraint violations.
- Candidate-source contribution, latency, and empty-result reasons are measurable and testable.

## REC-002 — Implement contextual scoring, diversity, and explanations

**Workstream:** Recommendation service  
**Depends on:** REC-001

**Requirements**

- Score preference fit, ingredient reuse, leftover opportunity, cuisine/protein/method diversity, time/effort, ratings/history, and optional nutrition targets.
- Avoid near duplicates and excessive repetition; return a small set with truthful human-readable reasons.

**Implementation**

- Build a versioned, configurable ranker with feature normalisation, weights, diversity re-ranking, top-k limits, and explanation templates derived from actual scored features.

**Definition of Done**

- Offline evaluation meets agreed eligibility, diversity, explanation-correctness, and relevance thresholds.
- Every displayed reason is reproducible from stored inputs and ranker version.

## API-010 — Expose non-mutating suggestions and feedback events

**Workstream:** API / Recommendations  
**Depends on:** REC-002, API-008

**Requirements**

- Return suggestions for an anchor meal, day, or week without modifying the plan.
- Record impression, accepted-to-plan, dismissed, and reason/position metadata with privacy controls.

**Implementation**

- Build request context validation, rate limits, recommendation response contracts, explanation payloads, add-to-plan handoff, and idempotent feedback endpoints.

**Definition of Done**

- API tests prove suggestion calls are read-only and only explicit add actions invoke plan mutations.
- Feedback events are deduplicated, permission-safe, measurable, and excluded from raw-content logging.

## FE-010 — Add contextual suggestions to planner and recipe views

**Workstream:** Web and mobile frontend  
**Depends on:** FE-008, MOB-002, API-010

**Requirements**

- Offer modes such as complete this day, balance this week, use what I am buying, use leftovers, similar alternatives, and household favourites.
- Display concise explanations and make add, dismiss, refresh, and personalisation controls explicit.

**Implementation**

- Build suggestion panels/cards, loading/empty/error states, reason display, add confirmation, dismissal, feedback instrumentation, and accessibility support.

**Definition of Done**

- Users can understand why a meal was suggested and add it only through a deliberate action.
- Usability and telemetry validation show no ambiguity between suggested and already planned meals.

## QA-005 — Evaluate recommendations online and offline

**Workstream:** QA / Data science  
**Depends on:** REC-002, API-010, FE-010

**Requirements**

- Measure eligibility precision, diversity, duplication, explanation correctness, latency, view-to-add, dismissal, and repeat engagement.
- Protect against popularity lock-in and misleading improvement caused by exposure bias.

**Implementation**

- Create labelled scenarios, offline ranking reports, event-quality checks, controlled beta analysis, guardrail metrics, and release thresholds.

**Definition of Done**

- Recommendation release gates and rollback criteria are approved before broad exposure.
- Dashboarded results can be segmented by suggestion mode without exposing private recipe text.

## ARC-002 — Prototype and decide on Neo4j adoption

**Workstream:** Architecture / Data  
**Depends on:** REC-002, QA-005, DB-007

**Requirements**

- Test whether explicit substitutions, food hierarchies, recipe–ingredient relationships, and leftover production/use materially improve results or explanations.
- Keep PostgreSQL authoritative and avoid persisting every pairwise semantic-similarity relationship.

**Implementation**

- Build a disposable outbox-fed Neo4j projection and compare relevant multi-hop queries against PostgreSQL/pgvector on quality, latency, cost, consistency, and maintainability.
- Document rebuild, deletion, monitoring, and ownership implications.

**Definition of Done**

- An ADR records adopt/defer/reject with measured evidence and a bounded operating model.
- If adopted, the graph is proven rebuildable and deletion propagation passes; if deferred, no production dependency remains.

---

# Phase 8 — Hardening, beta, launch, and continuous improvement

## SEC-003 — Complete security, privacy, and abuse hardening

**Workstream:** Security / Privacy  
**Depends on:** All feature tasks through ARC-002

**Requirements**

- Review authentication, tenant isolation, SSRF, file handling, signed URLs, model/prompt isolation, secrets, dependencies, logging, export, deletion, and retention.
- Validate protection against malicious files, abusive import volume, quota bypass, and cross-household inference leakage.

**Implementation**

- Perform threat-model refresh, automated scanning, targeted penetration testing, privacy review, abuse controls, key rotation, and remediation tracking.

**Definition of Done**

- No unresolved critical/high findings remain; accepted lower risks have owners and review dates.
- Security/privacy controls are verified in production-like staging and reflected in user-facing notices.

## QA-006 — Complete full-system functional and accessibility testing

**Workstream:** QA  
**Depends on:** FE-009, FE-010, QA-003, QA-004, QA-005, SEC-003

**Requirements**

- Cover every ingestion route through review, publish, search, planning, shopping, suggestions, export, and deletion.
- Validate supported browsers/devices, keyboard/screen reader use, focus order, zoom, contrast, target sizes, and error recovery.

**Implementation**

- Run end-to-end, exploratory, visual, localisation, accessibility, compatibility, and destructive-state test suites with production-like data volumes.

**Definition of Done**

- All MVP acceptance gates pass with no unresolved critical/high defects and agreed evidence for accessibility conformance.
- Known limitations are documented in product/support material and do not contradict launch claims.

## QA-007 — Complete performance, resilience, and recovery testing

**Workstream:** QA / Reliability  
**Depends on:** QA-006, OPS-002

**Requirements**

- Test API latency, search/vector queries, upload throughput, queue capacity, OCR/model provider latency, mobile sync, and cost at forecast and burst loads.
- Exercise database restore, object recovery, dead-letter replay, provider outage, network loss, worker crash, deployment rollback, and deletion propagation.

**Implementation**

- Build load/soak/chaos scenarios, define service objectives and error budgets, capture bottlenecks, and verify runbooks during game days.

**Definition of Done**

- Approved performance and reliability targets are met or documented capacity limits and controls are in place.
- Backup restoration, rollback, and failed-job replay succeed within agreed recovery objectives.

## PRD-002 — Run controlled beta and user acceptance testing

**Workstream:** Product / UX  
**Depends on:** QA-006, QA-007

**Requirements**

- Recruit a representative, consented beta cohort and measure import completion, correction burden, planning activation, suggestion use, failure severity, and support demand.
- Prioritise evidence by abandonment, frequency, consequence, and user impact rather than anecdote alone.

**Implementation**

- Configure feature flags/cohorts, feedback capture, support triage, interview tasks, analytics dashboards, issue severity rules, and go/no-go criteria.

**Definition of Done**

- Beta findings are resolved, explicitly accepted, or moved to a prioritised post-launch backlog.
- Product, engineering, security/privacy, support, and operations sign the launch readiness decision.

## OPS-003 — Prepare production, support, and incident readiness

**Workstream:** Operations  
**Depends on:** SEC-003, QA-007, PRD-002

**Requirements**

- Provide production infrastructure, capacity, monitoring, alerts, budgets, backups, restore schedules, provider quotas, support tools, and incident ownership.
- Define data/model/parser release controls and rollback procedures.

**Implementation**

- Finalise dashboards, alert thresholds, on-call routing, runbooks, status communications, support diagnostics, change calendar, and launch checklist.

**Definition of Done**

- Production smoke, security, backup, restore, rollback, and alert-routing rehearsals pass.
- Named owners can diagnose imports without viewing private content beyond authorised support workflows.

## OPS-004 — Execute staged production rollout

**Workstream:** Operations / Release  
**Depends on:** OPS-003

**Requirements**

- Release by controlled platform/cohort stages with observable go/no-go checkpoints and rapid rollback.
- Monitor authentication, imports by source class, errors, latency, queue health, cost, correction, planning, shopping, and suggestion guardrails.

**Implementation**

- Run migration rehearsal, production migration, canary deployment, synthetic checks, cohort ramp, store/web release steps, and incident watch.

**Definition of Done**

- Each rollout stage meets its guardrails before expansion and the final cohort is stable through the agreed observation window.
- Release notes, support material, ownership, and rollback artefacts are complete.

## OPS-005 — Establish continuous extraction and recommendation improvement

**Workstream:** Operations / Data quality  
**Depends on:** OPS-004

**Requirements**

- Continuously measure corrections, unresolved fields, source-class regressions, provider economics, recommendation acceptance/dismissal, diversity, and explanation feedback.
- Prevent user corrections or private content from becoming unrestricted model-training data without explicit policy and consent.

**Implementation**

- Schedule benchmark reruns for parser/OCR/model changes, drift reviews, correction taxonomy analysis, ranker evaluation, cost reviews, and quarterly roadmap decisions.

**Definition of Done**

- Every extraction or ranker release has a version, evaluation report, approval record, monitoring plan, and rollback path.
- Improvement priorities are tied to measured user outcomes and reviewed privacy constraints.

## PRD-003 — Govern post-launch expansion

**Workstream:** Product / Architecture  
**Depends on:** OPS-005, ARC-002

**Requirements**

- Evaluate pantry management, nutrition, multi-household features, grocery integrations, additional languages/source classes, and Neo4j only from measured demand and proven value.
- Preserve manual planning control, canonical PostgreSQL authority, evidence-backed extraction, and explainable suggestions as product invariants.

**Implementation**

- Maintain a quarterly outcome roadmap, architecture/risk review, capacity forecast, deprecation policy, and experiment register.

**Definition of Done**

- Each approved expansion has its own measurable hypothesis, privacy/security assessment, dependency map, and backlog before implementation begins.
- Unproven complexity is deferred with a recorded revisit trigger rather than entering production by default.

---

# Final product completion criteria

The initial product goal is achieved when:

- Manual, URL/structured-web, XML/feed, and photo/PDF inputs all produce the same versioned, evidence-backed `RecipeDraft` contract.
- Users can inspect and correct uncertain extraction, publish trusted recipes, search and organise their library, and export or delete their data.
- Users can manually plan meals for a day or week and generate a fully editable shopping list.
- The application can offer eligible, varied, contextual, explainable suggestions without ever changing the plan automatically.
- PostgreSQL remains the authoritative system, pgvector supplies proven semantic retrieval, and any Neo4j use is justified by measured multi-hop value and remains rebuildable.
- Security, privacy, accessibility, extraction-quality, resilience, cost, support, and operational acceptance gates pass in a staged production release.

