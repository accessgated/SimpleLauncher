# Contributing

Thank you for helping improve SimpleLauncher.

## Development Setup

1. Install the .NET SDK listed in [global.json](global.json).
2. Clone the repository.
3. Run `dotnet restore SimpleLauncher.slnx`.
4. Run `dotnet build SimpleLauncher.slnx`.
5. Run `dotnet test SimpleLauncher.slnx`.

## Coding Style

- Follow `.editorconfig`.
- Keep nullable reference types enabled.
- Prefer small, testable services over large UI classes.
- Validate user-selected files and paths before using them.
- Do not store secrets or PINs in plaintext.
- Keep Windows API calls isolated in infrastructure classes.

## Branch Naming

Use short, descriptive names:

- `feature/add-settings-window`
- `fix/config-recovery`
- `docs/update-roadmap`
- `security/pin-hardening`

## Commit Style

Use imperative commit messages:

```text
Add configuration backup service
Fix corrupted config recovery
Document shell replacement risks
```

## Pull Requests

Pull requests should include:

- A clear summary.
- Screenshots or recordings for UI changes.
- Tests for changed behavior.
- Notes about migration, security, or compatibility risks.

## Issues

Use the issue templates for bugs and feature requests. Include logs only after removing private paths, usernames, machine names, and other personal information.

## Review Process

Maintainers prioritize correctness, security, accessibility, and maintainability. A change may be declined if it adds risky kiosk behavior without a reversible recovery path.
