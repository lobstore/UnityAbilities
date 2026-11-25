# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- State-wide cancellation policies via `AllowCancel` and `MinTimeBeforeCancel`.
- Per-state interruption checks through `CanBeInterrupted` and `IsInterruptible`.
- Public controller API for starting, cancelling, and interrupting abilities.
- English and Russian user guides plus an agent-oriented API reference.

### Fixed
- `AbilityController.TryInterrupt` now interrupts the active ability when permitted.
- State machine completion is idempotent and exits the active state once.

## [0.1.0] - 2026-11-25

### This is the first release of *<LOGIYGames Abilities>*.
