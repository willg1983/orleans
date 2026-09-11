using System;
using System.Collections.Generic;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents an event to be written to a grain's event stream in storage.
/// </summary>
/// <param name="EventType">
/// The type name of the event, used to identify the correct type for deserialization on reads.
/// </param>
/// <param name="Data">
/// The serialized event payload.
/// </param>
/// <param name="EventId">
/// A unique identifier for the event, which can be used for idempotency checks and deduplication.
/// </param>
/// <param name="Metadata">
/// A dictionary of additional metadata to associate with the event, such as correlation or tracing information.
/// </param>
public readonly record struct EventWrite(
    string EventType,
    BinaryData Data,
    Guid EventId,
    IReadOnlyDictionary<string, object> Metadata);
