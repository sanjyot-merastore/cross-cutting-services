# Copilot Instructions for MeraStore Cross-Cutting Services

These instructions are mandatory for all changes in this repository.

## Repository Context

This solution uses clean architecture and is organized by responsibility:

- `src/MeraStore.Services.Cross.Cutting.Api`: HTTP surface, middleware, endpoint mapping.
- `src/MeraStore.Services.Cross.Cutting.Application`: use-cases, MediatR handlers, application services.
- `src/MeraStore.Services.Cross.Cutting.Domain`: contracts, entities, constants, domain exceptions.
- `src/MeraStore.Services.Cross.Cutting.Infrastructure`: EF Core persistence, repositories, external integrations.
- `sdk/MeraStore.Services.Logging.SDK`: reusable client SDK for consumers.
- `test/*`: architecture and unit tests.
- `aspire/*`: local distributed app orchestration.

Do not introduce cross-layer dependencies that break this direction:

- Api -> Application -> Domain
- Infrastructure -> Application/Domain (implementation side)
- Domain must remain framework-light and independent of infrastructure concerns.

## Non-Negotiable Development Rules

1. Keep changes minimal and scoped to the request.
2. Do not refactor unrelated files in the same commit.
3. Preserve existing public contracts unless explicitly asked to change them.
4. Keep naming and folder placement consistent with current patterns.
5. Do not bypass existing middleware, error handling, or tracing conventions.

## API Layer Rules

When adding or changing endpoints:

1. Implement endpoint classes using `IEndpoint` (`Domain/Interfaces/IEndpoint.cs`).
2. Follow existing endpoint grouping style used by `LoggingEndpoint`.
3. Keep routes versioned under `api/v1.0/...` unless a versioning change is requested.
4. Use explicit response metadata (`Produces`, status codes, summaries/descriptions).
5. Keep endpoint logic thin; move business behavior into Application services/handlers.

Startup and pipeline conventions in `Api/Program.cs` are authoritative:

1. Keep service registration order coherent: core/web API base, logging, application, infrastructure.
2. Keep middleware order intentional (tracing before error handling).
3. Do not remove startup migration behavior unless explicitly requested.

## Application Layer Rules

1. Use MediatR for request/command flows when behavior is a use-case.
2. Place features under the existing `Features/*` organization.
3. Keep handlers focused on orchestration and domain intent.
4. Keep infrastructure details out of Application logic.
5. Register new application services in `Application/ServiceRegistrations.cs`.

## Domain Layer Rules

1. Put shared constants and domain contracts in Domain.
2. Keep Domain free of persistence/web framework coupling whenever possible.
3. Reuse existing header and logging field constants before adding new literal strings.
4. If new constants are needed, group them under existing nested static classes.

## Infrastructure Layer Rules

1. Keep EF Core and repository implementations in Infrastructure.
2. Register repositories and persistence dependencies in `Infrastructure/ServiceRegistrations.cs`.
3. Use migrations for schema changes; do not rely on ad-hoc runtime schema edits.
4. Maintain connection-string usage through configuration (`LoggingDb`).

## SDK Rules

1. Keep SDK public API stable unless a breaking change is explicitly requested.
2. Keep endpoint constants centralized in SDK endpoint helper files.
3. Ensure client behavior remains resilient and consistent with existing error handling patterns.
4. If SDK contract changes are made, update integration app usage accordingly.

## Testing and Validation Rules

Before considering a change complete, run and pass:

1. `dotnet restore`
2. `dotnet build`
3. `dotnet test`

If a change impacts persistence, also validate migration behavior and startup migration execution path.

Do not mark work complete when tests/build are skipped unless the user explicitly asks to skip validation.

## CI and Repository Gate Awareness

Local hooks and CI in this repo enforce quality:

1. Pre-commit runs `dotnet build`.
2. Pre-push runs `dotnet test`.
3. GitHub workflow runs restore/build/test on pushes and PRs to main.
4. PR titles are validated for a JIRA-style key and a recognized change type.

When drafting PRs or summaries, use a compatible title pattern such as:

- `MERA-123: feat Add structured logging field mapping`

## Change Checklists

Use these checklists to avoid partial implementations.

### Adding a new API endpoint

1. Add endpoint class implementing `IEndpoint` under `Api/Endpoints/...`.
2. Map routes under a versioned group and include OpenAPI metadata.
3. Keep behavior in Application layer (service/handler), not in endpoint body.
4. Ensure endpoint is discoverable via existing reflection mapping.
5. Add/update tests where applicable.

### Adding a new use-case

1. Add command/query + handler under `Application/Features/...`.
2. Add/adjust repository interfaces in Application/Domain as needed.
3. Implement persistence changes in Infrastructure.
4. Register dependencies in service registration extensions.
5. Add tests for success and failure paths.

### Changing persistence schema

1. Update entity/configuration in Domain/Infrastructure.
2. Add EF migration with clear naming.
3. Validate migration application locally.
4. Confirm runtime startup migration path still succeeds.

### Changing SDK behavior

1. Update SDK endpoint/client logic and keep API shape intentional.
2. Verify integration app still compiles and behaves correctly.
3. Update versioning/package metadata only when requested by task scope.

## What to Avoid

1. Do not introduce new architectural patterns unless requested.
2. Do not move business rules into middleware/endpoints.
3. Do not hardcode environment-specific secrets or URLs.
4. Do not silently ignore build/test failures in final reports.

## Output Expectations for AI-Generated Changes

1. Always list modified files with a short reason.
2. Call out any assumptions explicitly.
3. Report validation commands executed and their outcome.
4. If validation could not be run, state why and what remains to verify.