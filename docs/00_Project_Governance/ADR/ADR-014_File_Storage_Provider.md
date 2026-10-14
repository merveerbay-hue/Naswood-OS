# ADR-014 — File Storage Provider Architecture

**Status:** Accepted  
**Date:** 2026-08-05  
**Module:** Platform / Shared File Storage  
**Deciders:** Product + Platform Engineering  

---

## Context

TASK-012 File Upload depends on a storage provider decision.  
`File_Storage.md` requires centralized object storage with Local (dev), Azure Blob, Amazon S3, and S3-compatible backends.

Cloud SDKs and credentials must not be invented inside Sprint 00 TASK work without an ADR.

---

## Decision

Naswood OS uses a provider-based file storage abstraction:

```
IFileStorage
├── LocalFileStorageProvider      (Development — implemented)
├── S3FileStorageProvider         (Production S3 / MinIO — interface + stub registration)
└── AzureBlobFileStorageProvider  (Optional Azure — interface + stub registration)
```

### Rules

1. All modules store/retrieve files only through `IFileStorage`.
2. Binary content never lives in business tables — metadata + storage key only.
3. Development default: **Local** filesystem under a configured root path.
4. Production target: **S3-compatible** (AWS S3 or MinIO self-hosted).
5. Azure Blob is optional and may be enabled via configuration.
6. Cloud provider **implementations** are deferred until credentials and environment wiring exist.
7. Provider selection is configuration-driven (`FileStorage:Provider` = `Local` | `S3` | `AzureBlob`).

### Abstraction surface (minimum)

- `UploadAsync`
- `DownloadAsync`
- `DeleteAsync`
- `ExistsAsync`
- `GetUriAsync` (time-limited / provider-specific where applicable)

---

## Consequences

### Positive

- TASK-012 can implement upload APIs against `IFileStorage` without cloud lock-in.
- Local provider unblocks development and integration tests.
- MinIO can satisfy S3-compatible production without code changes once S3 provider is completed.

### Negative / Follow-ups

- S3 and Azure providers are not production-ready until SDK packages and secrets are wired.
- Versioning, virus scan, and retention remain TASK-012 / File_Storage concerns.

---

## Related

- `docs/13_Design/99_Shared/File_Storage.md`
- `docs/14_Implementation/Sprint_00_Platform/TASK-012_File_Upload.md`
