---
description: "Use when changing SDK code under sdk, including client behavior, endpoint constants, and integration app usage."
applyTo: "sdk/**"
---

# SDK Instructions

Apply these rules for MeraStore.Services.Logging.SDK and its integration app.

## Public API Stability

1. Preserve SDK public method signatures unless a breaking change is explicitly requested.
2. Keep endpoint paths centralized in SDK endpoint helper files.
3. Keep response handling and error behavior consistent across methods.
4. Maintain resilient HTTP behavior patterns already in the SDK.

## Implementation Rules

1. Keep client transport concerns in client classes; avoid leaking transport details into consuming models.
2. Keep serialization choices consistent with current SDK usage.
3. Avoid hardcoding environment-specific URLs/secrets.
4. Update the integration app when contract changes are intentional.

## Versioning and Packaging

1. Do not change package version metadata unless requested by task scope.
2. If you introduce a compatibility-affecting change, call it out clearly in the final summary.

## Completion Checklist

1. SDK project builds successfully.
2. Integration app compiles after SDK changes.
3. Any changed endpoint constants remain aligned with API routes.
