# NEXORIS AI — Release Acceptance Checklist

Use this checklist for each candidate. CI success is necessary but not sufficient.

## Automated build and package

- [ ] Restore and Release build succeed on a clean Windows runner.
- [ ] Self-contained x64 publish succeeds.
- [ ] Installer compiles and expected installer file exists.
- [ ] SHA-256 checksum is generated and verified against the installer.
- [ ] GitHub Release contains the installer and checksum file.
- [ ] Release notes state supported OS/architecture, limitations, and version.

## Installation and application QA — real Windows machines required

- [ ] Install as a standard user without elevation where supported.
- [ ] Launch from Start Menu and optional desktop shortcut.
- [ ] Run System overview, Hardware inventory, Device health, Full diagnostic, Installed apps, and Cleanup preview.
- [ ] Save a report to a user-selected path; inspect it for unnecessary identifiers.
- [ ] Confirm cleanup preview does not delete or modify files.
- [ ] Confirm a WMI/provider failure does not crash the whole application.
- [ ] Test on supported Windows 10 and Windows 11 x64 versions actually claimed by the product.
- [ ] Uninstall; confirm app files are removed and user-created reports are not silently deleted.
- [ ] Test on a clean VM and representative physical hardware.
- [ ] Review SmartScreen/reputation behavior; checksum is not a substitute for code signing.

## Product, security, and legal

- [ ] Review source and dependencies for vulnerabilities and license obligations.
- [ ] Obtain a trusted code-signing certificate and sign installer/binaries, or explicitly disclose unsigned status.
- [ ] Product owner approves the privacy notice and final commercial license.
- [ ] Publish support/contact process, pricing, refund policy, and release/update policy.
- [ ] Confirm product claims match implemented and tested features.
- [ ] Verify accessibility, localization, error handling, and report privacy.
- [ ] Do not claim physical voltage/current measurement without compatible instruments.
- [ ] Do not advertise AI multi-agent, automatic repair, or broad hardware compatibility until implemented and tested.

## Release decision

Label a build **CI-built release candidate** when automated package checks pass. Label it **commercially released** only after real-machine, security, licensing, and support gates are completed and documented.
