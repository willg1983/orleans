namespace Orleans.EventSourcing;

/// <summary>
/// Represents an event that has been decoded from storage, pairing the deserialized event
/// payload with its position in the grain's event stream.
/// </summary>
/// <typeparam name="TEventBase">The base type of the event payload.</typeparam>
/// <param name="Event">The deserialized event payload.</param>
/// <param name="EventNumber">The position of this event within the grain's individual event stream.</param>
public readonly record struct DecodedEvent<TEventBase>(TEventBase Event, GrainEventNumber EventNumber)
    where TEventBase : notnull;

/// <summary>
/// Defines a mechanism for converting between domain event objects and the storage representations
/// used by <see cref="IGrainEventStorageProvider"/>.
/// </summary>
/// <remarks>
/// A default implementation that uses <c>IGrainStorageSerializer</c> for serialization will be
/// provided by the framework. Implementors can provide a custom implementation to support
/// alternative serialization formats or type-name mappings.
/// </remarks>
public interface IEventConverter
{
    /// <summary>
    /// Decodes a raw <see cref="EventRead"/> read from storage into a strongly-typed domain event.
    /// </summary>
    /// <typeparam name="TEventBase">The base type to decode the event payload as.</typeparam>
    /// <param name="read">The raw event data read from storage.</param>
    /// <returns>
    /// A <see cref="DecodedEvent{TEventBase}"/> containing the deserialized event payload and
    /// its position in the grain's event stream.
    /// </returns>
    DecodedEvent<TEventBase> DecodeEvent<TEventBase>(EventRead read) where TEventBase : notnull;

    /// <summary>
    /// Encodes a domain event into an <see cref="EventWrite"/> suitable for persisting to storage.
    /// </summary>
    /// <typeparam name="TEventBase">The base type of the event to encode.</typeparam>
    /// <param name="event">The domain event to encode.</param>
    /// <returns>An <see cref="EventWrite"/> containing the serialized event data and metadata.</returns>
    EventWrite EncodeEvent<TEventBase>(TEventBase @event) where TEventBase : notnull;
}
