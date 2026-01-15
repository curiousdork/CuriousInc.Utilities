# CuriousInc.Utilities

CuriousInc.Utilities is a multi-package toolbox of functional primitives, value objects, and Roslyn source generators that Curious Inc. reuses across services. All packages are modern .NET friendly (net8.0 → net10.0) while still shipping `netstandard2.0` builds for older consumers.

## Packages

| Package | Target Frameworks | Description |
| --- | --- | --- |
| `CuriousInc.Common.Functional` | `net8.0`, `net9.0`, `net10.0` | Core functional helpers (Option, Either, Result, value objects, and reusable extensions) designed for expressive domain modeling. |
| `CuriousInc.Utilities.Source.Generators` | `netstandard2.0` | Roslyn source generators that automate common DDD/functional boilerplate (records, discriminated unions, error wiring, etc.). Distributed as an analyzer package. |

## Key Features

- ✅ Strongly-typed primitives that reduce null/exception driven flow.
- ⚙️ Roslyn generators to keep domain models DRY.
- 📦 NuGet packages with source + symbol packages for first-class debugging.
- 🧪 Analyzer rule enforcement out of the box via `Microsoft.CodeAnalysis.Analyzers`.

## Installation