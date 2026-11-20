# Repository Conventions

This document is the source of truth for repository-wide conventions. AI-agent instruction files live in the root [`ai/`](ai/) directory:

| Area | Instructions |
| --- | --- |
| Agent workflow and prompt | [`ai/AGENT_PROMPT.md`](ai/AGENT_PROMPT.md) |
| Code rules and review checklist | [`ai/CODE_RULES.md`](ai/CODE_RULES.md) |
| Review examples (good/bad findings) | [`ai/REVIEW_EXAMPLES.md`](ai/REVIEW_EXAMPLES.md) |

The repository is a single .NET solution, [`LLDev.TI.CC2531.slnx`](LLDev.TI.CC2531.slnx), with two projects:

- [`LLDev.TI.CC2531/`](LLDev.TI.CC2531/) — the library, published to NuGet as `LLDev.TI.CC2531`
- [`LLDev.TI.CC2531.Tests/`](LLDev.TI.CC2531.Tests/) — unit tests (xUnit v3 + Moq)

This `AGENTS.md` applies to the whole repository tree. When instructions conflict, the file closer to the code wins.

## Core Principle

When multiple valid approaches exist, consistency with the existing codebase takes precedence over personal preferences and generic best practices.

## Architecture

A .NET library for communicating with ZigBee devices through the **Texas Instruments CC2531** USB dongle running ZStack firmware (ZNP — ZigBee Network Processor). The library acts as the ZigBee **network coordinator**: it creates and starts the network, controls permit join (with status LED feedback), and raises events for device announces and incoming end-device messages.

The library is the **only** component that opens the serial port to the dongle. Consumers use the public `INetworkHandler` (registered as a singleton via `AddZigBeeServices`) and never touch the serial port directly. Receiving messages is implemented; sending messages to devices is not yet implemented.

Internal layering of `LLDev.TI.CC2531/`:

- `Packets/` — ZNP packet model: `Packet`, `PacketHeader`, `PacketFactory`, plus `Incoming/` and `Outgoing/` packet types
- `Handlers/` — public `INetworkHandler`, serial-port handler, serial-port data handler, packet handler
- `Services/` — network coordinator service, packet receiver/transmitter, transactions, awaited-packet cache, critical section, command validation
- `Devices/` — network coordinator and network device abstractions
- `Models/`, `Enums/`, `Exceptions/`, `Configs/`, `Extensions/` — supporting types

## Conventions

- File-scoped namespaces, primary constructors where they fit, `Nullable` and `ImplicitUsings` enabled
- `.editorconfig` is enforced in the build (`EnforceCodeStyleInBuild`); both Debug and Release set `TreatWarningsAsErrors` with `WarningLevel 9999` — code must compile warning-free. Line endings are CRLF
- Approved dependencies are exactly the `PackageReference`s in the `.csproj` files (`System.IO.Ports`, `Microsoft.Extensions.Options.ConfigurationExtensions` for the library; xUnit v3, Moq, coverlet, `Microsoft.Extensions.Hosting` for tests). Do not add new dependencies without a measurable benefit
- `InternalsVisibleTo` is limited to `LLDev.TI.CC2531.Tests` and `DynamicProxyGenAssembly2` (Moq proxies)

## Testing

Run from the repository root, exactly as CI does in [`.github/workflows/UnitTests.yml`](.github/workflows/UnitTests.yml):

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

- Tests live in `LLDev.TI.CC2531.Tests/` and mirror the source folder structure (`Handlers/`, `Services/`, `Packets/`, ...)
- Test framework: xUnit v3 with the Microsoft.Testing.Platform runner (see `global.json`); mocking with Moq
- Add or update tests for every change

## Packaging

- The package version lives in `LLDev.TI.CC2531/LLDev.TI.CC2531.csproj` (`<Version>`); `README.md` and `LICENSE` are packed into the NuGet package
- `nuget.config` enforces a 96-hour `minPublishAge` cooldown on nuget.org packages — do not add package sources or settings that bypass it

## Restricted Patterns (repo-wide)

- Secrets, tokens, API keys, or connection strings in the repository
- Direct serial-port or dongle access outside the library's `ISerialPortHandler` — consumers go through `INetworkHandler`, tests mock the handler with Moq
- New dependencies, framework migrations, or large architectural rewrites without a measurable benefit
