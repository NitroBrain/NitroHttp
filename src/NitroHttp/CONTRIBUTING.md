# Contributing to NitroHttp

Thanks for your interest in contributing to NitroHttp.

This guide explains how to set up the project, make changes, and open high-quality pull requests.

## Code of conduct

- Be respectful and constructive in all discussions.
- Focus feedback on code and behavior, not people.

## Before you start

- Search existing issues/PRs to avoid duplicate work.
- For larger changes, open an issue first to align on scope.
- Keep contributions focused and minimal.

## Local setup

### Prerequisites

- .NET SDK with support for `net10.0`
- Git
- Linux, macOS, or Windows desktop environment

### Run locally

```bash
dotnet restore
dotnet run
```

### Build

```bash
dotnet build -c Release
```

## Project layout

- `MainWindow.axaml` – main UI layout
- `MainWindow.axaml.cs` – request/response logic, tabs, persistence
- `Helpers/` – utility classes (status text, byte formatting, store models)
- `scripts/` – publishing/packaging scripts

## Development guidelines

- Follow existing naming and formatting style.
- Prefer small, targeted changes over broad refactors.
- Avoid adding dependencies unless clearly needed.
- Do not include generated output directories (`bin/`, `obj/`) in commits.
- Keep UI and behavior changes consistent with current design.

## Testing and validation

Before opening a PR:

1. Build in Release mode:

   ```bash
   dotnet build -c Release
   ```

2. Run the app and verify your feature/bug fix manually.
3. Check that existing behavior is not broken (request send flow, tabs, persistence).

## Commit guidance

- Use clear commit messages in imperative style.
- Example: `Add request headers parsing for GET flow`

Suggested commit format:

```text
type(scope): short summary
```

Examples:
- `feat(request): add auth header support`
- `fix(history): prevent duplicate entries`
- `docs(readme): update packaging section`

## Pull request checklist

Include in your PR:

- What changed and why
- Screenshots/GIFs for UI changes
- Steps used to test locally
- Any known limitations

PR quality checklist:

- [ ] Scope is focused and minimal
- [ ] Project builds (`dotnet build -c Release`)
- [ ] Behavior tested manually
- [ ] Docs updated if behavior changed

## Reporting bugs

When opening a bug report, include:

- OS and version
- .NET SDK version (`dotnet --info`)
- Steps to reproduce
- Expected behavior
- Actual behavior
- Logs/error messages (if available)

## Feature requests

Please provide:

- Problem statement
- Proposed solution
- Alternatives considered
- UX notes (if UI-facing)

## Security

Do not open public issues for sensitive vulnerabilities.

Instead, contact maintainers privately and include:
- Impact summary
- Reproduction details
- Suggested mitigation

Thanks for helping improve NitroHttp.