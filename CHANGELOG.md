# Changelog

All notable changes to SimpleLauncher will be documented in this file.

## 0.1.3 - 2026-08-01

- Added administrator PIN changing in Settings.
- Require the current administrator PIN before changing it.
- Validate empty, short, and mismatched new PIN entries.
- Continue storing administrator PINs as PBKDF2-SHA256 hashes with random salts.
- Document the administrator PIN change workflow.

## 0.1.2 - 2026-08-01

- Rebuilt the prototype as a modern .NET 8 WPF project.
- Replaced legacy INI configuration with versioned JSON.
- Added legacy INI migration.
- Added PBKDF2-SHA256 PIN hashing.
- Added configuration backup and recovery foundations.
- Added logging, CI, CodeQL, documentation, and repository governance files.
