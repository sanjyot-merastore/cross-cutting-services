---
description: "Use when changing or adding tests under test, including architecture and unit test projects."
applyTo: "test/**"
---

# Test Instructions

Use these rules when modifying test projects.

## Test Design

1. Add focused tests for behavior changed by the implementation.
2. Prefer clear Arrange/Act/Assert structure.
3. Keep tests deterministic and isolated from external dependencies.
4. Cover success and failure paths where applicable.

## Architecture and Unit Coverage

1. Preserve architecture test intent and keep rules aligned with clean-architecture boundaries.
2. Keep unit tests close to concrete behavior contracts.
3. Use mocks/stubs only where external collaboration is required.

## Naming and Clarity

1. Use descriptive test names that state expected behavior.
2. Keep assertions explicit and minimal.
3. Avoid brittle assertions coupled to incidental implementation details.

## Completion Checklist

1. New or changed tests run with dotnet test.
2. Tests fail for the wrong behavior and pass for the intended behavior.
3. Removed or refactored behavior has corresponding test updates.
