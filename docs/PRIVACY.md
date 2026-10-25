# NEXORIS AI — Privacy Notice

**Status:** Draft for product-owner review before commercial publication  
**Last reviewed:** 2026-10-25

## Current desktop behavior

The current Windows desktop foundation is designed to run locally. It reads Windows system/runtime information and hardware/device metadata through Windows APIs and WMI; reads installed-application metadata from Windows uninstall registry entries; and scans selected temporary locations to display a bounded cleanup preview. It does not delete files. A text report is created only when the user chooses a save location.

The current foundation does not implement account sign-in, telemetry, remote diagnostic uploads, or cloud AI calls.

## Diagnostic report contents

Reports can contain computer configuration, operating-system details, device names/status, installed-application metadata, and timestamps. Review a report before sharing it. Avoid posting private reports publicly.

The application is intended to avoid collecting serial numbers, MAC addresses, and unique hardware identifiers in inventory output. This must be re-checked against every release build.

## Storage and transmission

The current foundation saves a report only to a destination selected by the user and does not intentionally transmit diagnostic data to NEXORIS servers. Future online, account, update, support-upload, or AI features would change this assessment and require this notice to be revised before release.

## Retention and security

NEXORIS AI does not operate a server-side diagnostic-report store in the current implementation. Users control files saved locally. Download the installer from the official project release page and verify its published SHA-256 checksum. A checksum does not prove publisher identity; builds are unsigned unless a release explicitly states otherwise.

## Contact and changes

Use the project issue tracker for privacy questions, without posting private reports or secrets: https://github.com/akvisomr-eng/NEXORIS-AI/issues

This draft describes the current implementation, not roadmap features. The product owner must review and approve it, including applicable jurisdictional requirements, before commercial publication.
