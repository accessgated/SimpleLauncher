# Security Policy

## Supported Versions

Security fixes are provided for the latest stable release. During early development, security issues are fixed on `main` before the next release.

## Reporting a Vulnerability

Do not open a public issue for a suspected vulnerability.

Email the maintainers or use GitHub private vulnerability reporting when available. Include:

- Affected version or commit.
- Steps to reproduce.
- Expected and actual behavior.
- Security impact.
- Any proof-of-concept details needed to validate the report.

## Disclosure

Maintainers will acknowledge valid reports, investigate impact, prepare a fix, and publish an advisory when appropriate.

## Security Principles

- PINs must never be stored in plaintext.
- Configuration must be validated and recoverable.
- Shell and taskbar changes must be reversible.
- Privilege escalation must not be required for normal launcher use.
- Windows API usage must stay isolated and auditable.
