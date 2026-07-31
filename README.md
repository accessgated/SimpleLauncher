# SimpleLauncher

[![CI](https://github.com/accessgated/SimpleLauncher/actions/workflows/ci.yml/badge.svg)](https://github.com/accessgated/SimpleLauncher/actions/workflows/ci.yml)
[![CodeQL](https://github.com/accessgated/SimpleLauncher/actions/workflows/codeql.yml/badge.svg)](https://github.com/accessgated/SimpleLauncher/actions/workflows/codeql.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

SimpleLauncher is a standalone fullscreen Windows launcher for family computers, schools, public kiosks, gaming stations, and custom shell-replacement scenarios.

The project is intentionally independent. AccessGate or any other product may launch SimpleLauncher, but SimpleLauncher does not depend on them.

## Features

- Fullscreen Windows launcher with high-DPI WPF UI.
- JSON configuration stored in the user profile.
- Automatic migration from the legacy `Shortcuts.ini` prototype format.
- Add applications from the UI.
- Search, favorites-first ordering, categories, and configurable icon size.
- Optional PIN protection backed by PBKDF2-SHA256 hashing.
- Settings protection, configuration backup, import, and export service foundations.
- Friendly error handling for missing icons, missing backgrounds, invalid config, and launch failures.
- File logging under `%AppData%\SimpleLauncher\Logs`.
- Reversible taskbar hiding for kiosk-style sessions.

## Screenshots

Screenshots will be added as the v1.0 UI stabilizes.

## Installation

Download a release package from GitHub Releases, extract it, and run `SimpleLauncher.exe`.

For startup at sign-in, create a shortcut to `SimpleLauncher.exe` in:

```text
%AppData%\Microsoft\Windows\Start Menu\Programs\Startup
```

Shell replacement is planned as an explicit, documented administrator workflow. It is not enabled by default.

## Building

Install the .NET 8 SDK specified in [global.json](global.json), then run:

```powershell
dotnet restore SimpleLauncher.sln
dotnet build SimpleLauncher.sln --configuration Release
dotnet test SimpleLauncher.sln --configuration Release
```

## Configuration

SimpleLauncher stores configuration at:

```text
%AppData%\SimpleLauncher\launcher.json
```

The application creates a safe default configuration on first launch and automatically migrates a legacy `Shortcuts.ini` file if one exists beside the executable.

## Usage

- Use **Add** to select an application or shortcut.
- Use search to filter by application name or category.
- Protected apps and exit actions require the configured PIN.
- Settings create a backup before configuration changes.

The first-run default PIN is `1234`. Change it before deploying to children, students, guests, or public users.

## Troubleshooting

Logs are written to:

```text
%AppData%\SimpleLauncher\Logs\simplelauncher.log
```

If configuration is corrupted, SimpleLauncher backs it up and starts with safe defaults.

## Roadmap

See [ROADMAP.md](ROADMAP.md).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## Security

See [SECURITY.md](SECURITY.md). Please do not report vulnerabilities in public issues.

## License

SimpleLauncher is released under the [MIT License](LICENSE). MIT fits this project because it is permissive, familiar to Windows/.NET contributors, and friendly to schools, families, hobbyists, and commercial kiosk deployments.
