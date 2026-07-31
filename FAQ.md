# FAQ

## Does SimpleLauncher require AccessGate?

No. SimpleLauncher is completely standalone.

## Where is configuration stored?

Configuration is stored at `%AppData%\SimpleLauncher\launcher.json`.

## Is the PIN encrypted?

The PIN is not stored directly. SimpleLauncher stores a PBKDF2-SHA256 hash with a random salt.

## Can SimpleLauncher replace the Windows shell?

That is planned, but it must be explicit, reversible, and documented. The app currently focuses on standalone fullscreen launcher behavior.

## What happens if the configuration is corrupted?

SimpleLauncher backs up the corrupted file and starts with safe defaults.
