---
description: "Use when changing projects under src (Api, Application, Domain, Infrastructure) in this clean-architecture solution."
applyTo: "src/**"
---

# Source Layer Instructions

Follow these rules in addition to workspace-wide instructions.

## Architecture Boundaries

1. Preserve dependency flow: Api -> Application -> Domain.
2. Keep Domain independent from infrastructure/web concerns.
3. Keep Infrastructure as implementation details for persistence/integrations.
4. Avoid cross-feature coupling; prefer explicit interfaces over direct concrete usage.

## API Changes

1. Implement endpoints with the IEndpoint contract and keep endpoint handlers thin.
2. Keep routes versioned under api/v1.0 unless explicitly requested otherwise.
3. Add OpenAPI metadata (summary, description, produces/status codes).
4. Preserve middleware intent and ordering used in Program startup.
5. Do not move business logic into endpoint classes or middleware.

## Application Changes

1. Use MediatR commands/queries and handlers for use-case behavior.
2. Place features under existing Features organization.
3. Keep handlers orchestration-focused; enforce domain intent and validation.
4. Keep infrastructure specifics out of Application code.
5. Register application services in Application/ServiceRegistrations.cs.

## Domain Changes

1. Reuse constants and contracts from Domain before introducing new literals.
2. Keep entities and contracts framework-light.
3. If adding constants, group them under existing nested classes by concern.
4. Prefer additive, non-breaking updates to public contracts.

## Infrastructure Changes

1. Keep repository and EF Core implementation details in Infrastructure.
2. Register persistence/repositories in Infrastructure/ServiceRegistrations.cs.
3. Use migrations for schema changes; avoid ad-hoc runtime schema manipulation.
4. Keep connection string usage configuration-driven (LoggingDb).

## Completion Checklist

1. Build passes for src changes.
2. Tests covering modified behavior are added or updated.
3. New/updated endpoints include response metadata and proper route grouping.
4. Persistence changes include migration verification.
