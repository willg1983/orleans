using System;
using System.Collections.Generic;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents an event read from a grain's event stream in storage.
/// </summary>
/// <param name="EventType">
/// The type name of the event, used to identify the correct type for deserialization.
/// </param>
/// <param name="Data">
/// The serialized event payload.
/// </param>
/// <param name="EventNumber">
/// The position of this event within the grain's individual event stream.
/// </param>
/// <param name="Sequence">
/// The position of this event within the global event log, enabling total ordering across all grains.
/// </param>
/// <param name="EventId">
/// A unique identifier for the event, which can be used for idempotency checks and deduplication.
/// </param>
/// <param name="Metadata">
/// A dictionary of additional metadata associated with the event, such as correlation or tracing information.
/// </param>
public readonly record struct EventRead(
    string EventType,
    BinaryData Data,
    GrainEventNumber EventNumber,
    GlobalEventLogPosition Sequence,
    Guid EventId,
    IReadOnlyDictionary<string, object> Metadata);
