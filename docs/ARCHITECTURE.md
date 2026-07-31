# Architecture

## Current Prototype Problems

- One WinForms class owned UI, configuration, launching, PIN prompts, shell changes, keyboard hooks, icon loading, and shutdown.
- Configuration used INI files beside the executable, which is brittle and often not writable.
- The PIN was hardcoded as plaintext.
- User-selected paths were launched without enough validation.
- Windows API calls were mixed into presentation code.
- Shutdown used forced process termination.
- Missing files produced blocking error dialogs or crashes instead of recovery.
- Documentation instructed users to manually edit config files and replace assets by filename.

## New Architecture

```text
src/SimpleLauncher
  App
  Presentation
  Models
  Services
  Infrastructure
    Configuration
    Migration
    Security
    Windows
  Themes
  Logging
tests/SimpleLauncher.Tests
docs
.github
```

## Design Decisions

- WPF is used for modern Windows desktop UI, high-DPI support, styling, and future accessibility work.
- JSON configuration replaces INI and is stored in `%AppData%`.
- Configuration writes are atomic through a temporary file.
- Legacy INI migration runs once when JSON config does not exist.
- PIN storage uses PBKDF2-SHA256 with random salt and fixed-time verification.
- Windows shell operations are isolated in `WindowsShellService`.
- Process launching is isolated and validates paths before use.
- Logging is available from startup before external dependencies are introduced.
- Tests focus first on migration and security-critical hashing behavior.

## Migration Strategy

1. Look for existing JSON configuration.
2. If JSON exists, load and normalize it.
3. If JSON is corrupted, back it up and start safely.
4. If JSON does not exist, look for legacy `Shortcuts.ini` beside the executable.
5. Convert shortcut sections to JSON application entries.
6. Save the new JSON file under `%AppData%\SimpleLauncher`.

## Security Notes

SimpleLauncher is a launcher, not a hardened operating-system security boundary. Kiosk and parental-control features reduce casual access, but Windows administrator accounts, safe mode, external boot media, and OS-level policy gaps can bypass application-layer controls.
