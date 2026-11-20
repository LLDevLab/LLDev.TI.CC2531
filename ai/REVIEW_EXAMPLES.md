# C# Review Examples

> **Scope:** these examples illustrate review findings in the C# solution of this repository (the `LLDev.TI.CC2531` library and `LLDev.TI.CC2531.Tests` tests).

## Good examples
## Example 1
Severity: Major

Location: PacketReceiverTransmitterService.cs:58

Problem: Opens a raw `System.IO.Ports.SerialPort` instead of using the existing `ISerialPortHandler`.

Why it matters: Most of the library goes through the injected serial-port handler; a second direct serial-access pattern breaks the layering and bypasses the awaited-packet cache and critical-section handling.

Suggested Fix: Resolve `ISerialPortHandler` through DI and send/receive via its existing methods.

Reason: Consistency with repository standards.

## Example 2
Severity: Critical

Location: SerialPortDataHandler.cs:31

Problem: A catch-all `catch (Exception)` swallows dongle-disconnect errors and the receive loop dies silently, so no device events are raised anymore.

Why it matters: A disconnected or reset dongle would look like a quiet network; users get no error and no recovery.

Suggested Fix: Handle the expected `SerialPortException` with the existing logging/restart pattern and let unexpected exceptions propagate.

## Bad examples
## Bad Example 1
Problem: Use MediatR for this operation.

Reason: The repository does not currently use MediatR.

## Bad Example 2
Problem: Convert all classes to records.

Reason: Large-scale modernization is outside the scope of code review.
