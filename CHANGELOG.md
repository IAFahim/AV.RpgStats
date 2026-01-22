# Changelog

All notable changes to the AV.RpgStats package will be documented in this file.

## [1.0.0] - 2025-01-13

### Added
- Initial release of AV.RpgStats system
- `RpgStatsDictionary` - Core stat management with modifier support
- `RpgStatActivator` - Component for applying temporary stat modifiers
- `IRpgStatsMap` - Interface for stat map implementations
- Target system integration with `AV.Lifetime`
- Custom PropertyDrawer for `RpgStatsModEntry` with target pinging
- DOD-compliant architecture (Data-Logic-Extension separation)
- Support for Add, Subtract, Multiply, Divide operations
- Automatic inverse modifier calculation for cleanup

### Changed
- Renamed `RpgStatModifiers` to `RpgStatActivator` for clarity
- Cleaned up folder structure
- Moved demo assets to `Demos~` folder

### Technical Details
- Uses Unity's `TryGetComponent<T>` for interface resolution
- Follows Data-Oriented Design principles
- Burst-compatible logic layer
- Zero-allocation stat operations
