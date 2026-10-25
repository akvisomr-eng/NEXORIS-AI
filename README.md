# NEXORIS AI

**Intelligent Computer Diagnostics & Maintenance** — Windows-first and offline-first.

NEXORIS currently provides a small, read-only Windows desktop foundation: system overview, best-effort WMI hardware inventory, installed-application inventory, non-destructive temporary-file cleanup preview, and text report export.

## Implemented foundation

- **System overview:** OS/runtime and architecture metadata plus process and uptime information.
- **Hardware inventory:** motherboard/baseboard, BIOS/firmware, CPU, graphics adapters, memory modules, storage, physical network adapters, and Plug-and-Play device inventory through Windows Management Instrumentation (WMI).
- **Device health assessment:** summarizes Windows-reported Plug-and-Play status codes, separates disconnected devices from reported problems, and suggests safe next checks. It is not a physical hardware self-test and does not repair devices.
- **Full diagnostic report:** combines system overview, hardware inventory, and device health into one locally generated report; review device details before sharing it.
- **Application inventory:** reads uninstall registry metadata; never runs uninstall strings.
- **Cleanup preview:** lists a bounded number of old files from temporary locations; never deletes anything.
- **Reports:** user chooses the path for a local text report.
- **Build pipeline:** Windows GitHub Actions build, self-contained x64 publish, Inno Setup installer, and SHA-256 checksum artifact.
- **Versioned release workflow:** tag/manual-triggered build designed to create a **draft** GitHub Release with the installer, checksum, portable ZIP, and release documents. A draft is not a public or commercially approved release.

## Compatibility policy

This is **best-effort detection, not a promise of universal compatibility**. Actual coverage depends on Windows edition/build, architecture, drivers, firmware/SMBIOS data, permissions, and each device's exposed interfaces. The current build targets **Windows 10/11 x64 using .NET 8**; older Windows releases and ARM64 need separate target/runtime decisions and real-device tests before being claimed as supported. Unknown or unavailable values must be reported as unavailable, not fabricated.

Vendor-specific temperature sensors, fan control, embedded-controller registers, SMART/NVMe health details, and motherboard electrical rails may need supported drivers, vendor interfaces, or compatible external instruments. Software alone cannot measure physical voltage/current on arbitrary hardware.

## Safety and privacy

- Read-only diagnostics by default; cleanup is preview-only.
- No automatic deletion of user files.
- No arbitrary command execution, AI-generated command execution, or uninstall-string execution.
- No disabling Defender, firewall, Windows Update, or other security services.
- Reports are saved only when the user explicitly chooses a destination.
- No telemetry, account requirement, or cloud API call is implemented in this desktop foundation.
- Any future repair workflow must require explicit consent, a strict allowlist, a clear impact preview, an audit trail, and post-action verification.
- See [Privacy Notice (draft)](docs/PRIVACY.md) and [Commercial License Decision (draft)](docs/COMMERCIAL-LICENSE-DRAFT.md).

## Build and run

Open `NexorisAI/NexorisAI.csproj` on a Windows development machine with the .NET 8 SDK, then build in Release configuration. GitHub Actions is configured to publish a self-contained x64 preview and package it using Inno Setup. Installer/build success must be confirmed from the actual workflow run before distributing an artifact.

## Commercial release gate

Follow [Release Acceptance Checklist](docs/RELEASE-CHECKLIST.md). A successful CI build is not a substitute for real-machine install/launch/uninstall tests, security/dependency review, approved license terms, an approved privacy notice, support policy, and code-signing decision. The current app does not implement AI multi-agent orchestration, automatic repair, subscription/license activation, or physical electrical measurement.

## Roadmap (not yet implemented)

1. Test matrix for supported Windows builds, physical devices, VMs, permissions, and non-English environments.
2. More robust PnP/SetupAPI and storage-health discovery with explicit support/unknown states.
3. Safe repair planner, allowlisted operations, user confirmation, audit log, and post-action verification.
4. Optional AI multi-agent orchestration behind provider, privacy, rate-limit, and safety gates; disabled until tested.
5. Technician workflows and compatible external-instrument adapters for electrical measurements.

## Status

**Development preview / release-candidate preparation — not production-certified.** No claim of universal motherboard/peripheral support, successful real-machine installer validation, or hardware test coverage is made until backed by evidence.
