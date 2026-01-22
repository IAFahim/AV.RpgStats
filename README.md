# AV.RpgStats

A high-performance, Data-Oriented Design (DOD) RPG stat system for Unity. Provides a clean architecture for managing character stats with temporary modifiers, targeting systems, and editor tooling.

## Features

- **DOD Architecture**: Clean separation of Data, Logic, and Extension layers
- **Burst-Compatible**: Static logic classes designed for high performance
- **Target System**: Apply stat modifiers to different targets (Owner, Source, Target, Custom)
- **Editor Tooling**: Custom PropertyDrawer with target pinging
- **Type-Safe**: Compile-time safety with ScriptableObject-based stat definitions
- **Modifier Operations**: Add, Multiply, Subtract, Divide with inverse operations for cleanup

## Installation

Install all required packages via Unity Package Manager or add to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.av.lifetime": "1.0.0",
    "com.av.variablerpg": "1.0.0",
    "com.av.cancelfoldout": "1.0.0",
    "com.av.dictionaryvisualizer": "1.0.0",
    "com.unity.entities": "1.0.0"
  }
}
```

Copy the `AV.RpgStats` folder to your Unity project's `Assets` directory.

## Quick Start

### 1. Define Your Stats

Create a `RpgStatScript` ScriptableObject for each stat:

```
Assets/YourProject/Stats/
├── Health.asset
├── Mana.asset
└── Speed.asset
```

### 2. Create a Stats Dictionary

Add a `RpgStatsDictionary` component to your character GameObject:

```csharp
// The dictionary holds the current stat values
var statsDict = GetComponent<RpgStatsDictionary>();
```

### 3. Apply Temporary Modifiers

Add a `RpgStatActivator` component to apply modifiers when enabled:

```csharp
// Example: Apply +10 Health when a powerup is collected
// Modifiers are automatically removed when the component is disabled
```

## Architecture

### Data Layer (Structs)

- `RpgStat` - Pure stat value with base value and modifiers
- `RpgStatsModEntry` - Serializable entry for stat modifiers with targeting

### Logic Layer (Static Classes)

- `RpgStatsDictionary` - Core stat management and modifier application
- Uses `TryGetComponent<T>` for interface resolution

### Extension Layer (Extension Methods)

- `RpgStatsMapExtensions` - Convenience methods for stat manipulation

## Usage Examples

### Applying Modifiers via Code

```csharp
// Get the stats map
var statsMap = GetComponent<IRpgStatsMap>();

// Apply a +10 modifier to Health
statsMap.Apply(healthId, new RpgStatModifier(EOperation.Add, 10));

// Apply a 20% multiplier to Speed
statsMap.Apply(speedId, new RpgStatModifier(EOperation.Multiply, 1.2f));
```

### Using the Target System

```csharp
// RpgStatActivator with targeting:
// - Self: Apply to own stats
// - Owner: Apply to the object that owns this component
// - Source: Apply to the object that triggered an event
// - Target: Apply to the target of an action
// - Custom0/1: User-defined targets
```

## API Reference

### IRpgStatsMap

```csharp
public interface IRpgStatsMap
{
    bool IsDictionaryDirty { get; }
    void Apply(int id, RpgStatModifier modifier);
    bool TryGet(int id, out RpgStat stat);
}
```

### RpgStatsDictionary

```csharp
public class RpgStatsDictionary : MonoBehaviour, IRpgStatsMap
{
    // Apply a modifier to a stat
    public void Apply(int id, RpgStatModifier modifier);

    // Get current stat value
    public bool TryGet(int id, out RpgStat stat);

    // Log all stats to console
    [ContextMenu("Log Pretty")]
    public void Log();
}
```

### RpgStatActivator

```csharp
public class RpgStatActivator : MonoBehaviour
{
    [SerializeField] private RpgStatsModEntry[] rpgStatsModEntry;
    // Modifiers are applied OnEnable and removed OnDisable
}
```

## Modifier Operations

| Operation | Description | Has Inverse |
|-----------|-------------|-------------|
| `Add` | Add value to base | Yes (Subtract) |
| `Subtract` | Subtract value from base | Yes (Add) |
| `Multiply` | Multiply base by value | Yes (Divide) |
| `Divide` | Divide base by value | Yes (Multiply) |

## Performance Considerations

- **Zero Allocation**: Logic layer uses primitive types only
- **Burst Compatible**: Static methods can be used in Burst jobs
- **Caching**: Target resolution uses lazy caching for efficiency
- **Struct Layout**: Sequential memory layout for better cache locality

## Requirements

- Unity 2022.3 or later
- `AV.Lifetime` package
- `Variable.RPG` package
- `AV.CancelFoldout` package
- `AV.DictionaryVisualizer` package
- `Unity Entities` (BovineLabs.Core)

## License

MIT License

## Contributing

This package follows Data-Oriented Design principles. When contributing:
1. Keep the Data-Logic-Extension separation
2. Use `in` modifiers for read-only parameters
3. Return `void` or `bool` from logic methods
4. Use `out` parameters for results

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.
