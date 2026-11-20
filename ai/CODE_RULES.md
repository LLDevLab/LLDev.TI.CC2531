# C# Code Rules
Rules every AI agent applies when writing code and when reviewing changes.

> **Scope:** these rules apply to the whole C# solution in this repository — the `LLDev.TI.CC2531` library and the `LLDev.TI.CC2531.Tests` test project.

Conventions defined in the repo-wide [AGENTS.md](../AGENTS.md) take precedence over these rules.

Only report or fix issues that are actionable and justified.

Avoid subjective preferences.

## Priorities
Evaluate code in the following order:

Correctness
Security
Architecture consistency
Maintainability
Readability
Performance
Style consistency

## Verify
Apply these checks to your own changes before submitting and to any changes you review.

### Correctness
Check for:

- Logic errors
- Null reference risks
- Invalid assumptions
- Race conditions (the serial-port receive path is multi-threaded — check shared state)
- Resource leaks (serial port, timers, event subscriptions)

### Security
Check for:

- Injection vulnerabilities
- Sensitive data exposure (e.g. logging raw dongle traffic with addresses)
- Insecure configuration
- Missing authorization checks

### Architecture
Check for:

- Layer boundary violations (only `ISerialPortHandler` opens the serial port; consumers use `INetworkHandler`)
- Dependency direction violations (public API in `Handlers/`, protocol details in `Packets/` and `Services/`)
- Tight coupling
- Duplicate logic (packet parsing, transaction handling)

### Consistency
Check for:

- Naming consistency
- Folder placement consistency
- Existing pattern usage
- Existing abstraction usage

### C# Practices
Check for:

- Nullable reference types
- Correct async/await usage
- CancellationToken support where the existing code uses it
- Proper disposal patterns
- Collection usage
- LINQ correctness
- Object initializers instead of sequential property assignment statements after construction (`var test = new TestObject(...) { Prop1 = 1, Prop2 = 2 };`, not `var test = new TestObject(...);` followed by `test.Prop1 = 1;` statements)

### Unit tests
#### Naming convention
- Async suffix should be present only at the end of the test method name

#### Mock usage
- `mock.Setup(...)` only to stub methods returning a real value (`Returns`/`ReturnsAsync`) or to make them throw (`Throws`/`ThrowsAsync`); for `void` and plain `Task`-returning methods verify execution with `mock.Verify(...)` instead

## Severity Levels
Classify findings using these levels, whether the finding comes from self-review or from reviewing changes by others.

### Critical
Issues that may cause:

- Security vulnerabilities
- Data corruption
- Production failures (e.g. losing the serial-port receive loop)
- Major architectural violations

### Major
Issues that reduce:

- Maintainability
- Consistency
- Extensibility

### Minor
Issues that affect:

- Readability
- Naming
- Small consistency problems

### Info
Optional improvements.
