# AV.RpgStats

![Header](Documentation~/documentation_header.svg)

[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-000000.svg?style=flat-square&logo=unity)](https://unity.com)
[![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE.md)

Data-oriented RPG stat system with high-performance modifier support.

## ✨ Features

- **Stat System**: Base Value + Modifiers = Current Value.
- **Modifiers**: Add, Multiply, etc., with support for Timer-based expiration.
- **Targeting**: Apply modifiers to Self, Target, Source, etc.
- **Dictionary Lookup**: Efficient ID-based stat retrieval.

## 📦 Installation

Install via Unity Package Manager (git URL).

### Dependencies
- **Variable.RPG** (NuGet)
- **AV.Lifetime**
- **AV.CancelFoldout**
- **AV.DictionaryVisualizer**

## 🚀 Usage

1. Create `RpgStatScript` assets (Health, Mana).
2. Add `RpgStatsDictionary` to character.
3. Use `RpgStatActivator` to apply modifiers on Enable/Disable.

## ⚠️ Status

- 🧪 **Tests**: Missing.
- 📘 **Samples**: Included in `Samples~`.

## 🔍 Deep Dive

### Internal Architecture
The system uses a Data-Oriented approach where data (Structs) is separated from Logic (Static Classes).

```mermaid
graph TD
    subgraph Data Layer
        S[RpgStat Struct]
        M[RpgStatModifier]
        D[RpgStatsDictionary]
    end

    subgraph Logic Layer
        L[RpgStatLogic]
        E[RpgStatsMapExtensions]
    end

    subgraph Unity Layer
        Mono[RpgStatsDictionary MonoBehaviour]
        Activator[RpgStatActivator]
    end

    Mono -->|Contains| D
    D -->|Stores| S
    Activator -->|Adds/Removes| M
    M -->|Modifies| S
    L -->|Calculates| S
```