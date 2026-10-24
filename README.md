# NEXORIS AI

Intelligent Computer Diagnostics & Maintenance.

Windows-first, offline-first diagnostics with safe, read-only inspection by default.

## Compatibility strategy

Detect Windows product/version/build/architecture and hardware capabilities using supported Windows APIs, WMI/CIM, Plug-and-Play device data, and firmware-exposed SMBIOS information. Motherboard sensors, fan controls, electrical readings, and vendor-specific functions require compatible drivers, vendor interfaces, or external instruments. Missing readings must be reported as unavailable, never fabricated.

## Safety

- Read-only diagnostics by default.
- Cleanup preview only; never automatically delete user files.
- Never execute arbitrary registry uninstall strings or AI-generated commands.
- Repairs require explicit approval, validated allowlists, audit logs, and post-action verification.
- Never disable Defender, firewall, Windows Update, or security services as an optimization.

## Status

Development foundation only. Compatibility and commercial readiness require CI, installer validation, and tests on real Windows versions and hardware.