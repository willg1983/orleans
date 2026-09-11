using Orleans.Runtime;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents an update delivered to a subscriber of <see cref="IGrainEventProvider"/>.
/// </summary>
/// <remarks>
/// Use a <see langword="switch"/> expression or pattern matching to handle each concrete subtype:
/// <list type="bullet">
///   <item><see cref="GrainEvent{TEventBase}"/> — an event payload.</item>
///   <item><see cref="GrainEventNotification"/> — a notification that an event occurred (without the payload).</item>
///   <item><see cref="Checkpoint"/> — a progress marker for durable cursor tracking.</item>
///   <item><see cref="CaughtUp"/> — the subscription has processed all available historical events.</item>
///   <item><see cref="FallenBehind"/> — the subscription is lagging and may not be real-time.</item>
/// </list>
/// </remarks>
public abstract record EventStreamUpdate
{
    private EventStreamUpdate() { }

    /// <summary>
    /// Indicates that the subscription has caught up to the latest available event and is now
    /// delivering events in real time.
    /// </summary>
    public sealed record CaughtUp : EventStreamUpdate
    {
        private CaughtUp() { }

        /// <summary>
        /// Gets the singleton instance of <see cref="CaughtUp"/>.
        /// </summary>
        public static CaughtUp Instance { get; } = new();
    }

    /// <summary>
    /// Indicates that the subscription is lagging behind the head of the event log and is no
    /// longer processing events in real time.
    /// </summary>
    public sealed record FallenBehind : EventStreamUpdate
    {
        private FallenBehind() { }

        /// <summary>
        /// Gets the singleton instance of <see cref="FallenBehind"/>.
        /// </summary>
        public static FallenBehind Instance { get; } = new();
    }

    /// <summary>
    /// A progress marker emitted periodically while the subscription processes events that do not
    /// match the subscriber's filter.  Subscribers should persist the <see cref="Position"/> to
    /// enable resumption without replaying the entire event history.
    /// </summary>
    /// <param name="Position">The current position in the global event log.</param>
    public record Checkpoint(GlobalEventLogPosition Position) : EventStreamUpdate;

    /// <summary>
    /// Notifies the subscriber that an event was emitted by a grain without including the event
    /// payload.  This is a subtype of <see cref="Checkpoint"/> so the position is always available.
    /// </summary>
    /// <param name="Position">The position of the event in the global event log.</param>
    /// <param name="EventGrainId">The identifier of the grain that produced the event.</param>
    /// <param name="GrainEventNumber">The position of the event within the grain's own event stream.</param>
    public record GrainEventNotification(
        GlobalEventLogPosition Position,
        Runtime.GrainId EventGrainId,
        GrainEventNumber GrainEventNumber)
        : Checkpoint(Position);

    /// <summary>
    /// Delivers an event payload emitted by a grain to the subscriber.
    /// </summary>
    /// <typeparam name="TEventBase">The base type of the event payload.</typeparam>
    /// <param name="Position">The position of the event in the global event log.</param>
    /// <param name="Event">The deserialized event payload.</param>
    /// <param name="EventGrainId">The identifier of the grain that produced the event.</param>
    /// <param name="GrainEventNumber">The position of the event within the grain's own event stream.</param>
    public sealed record GrainEvent<TEventBase>(
        GlobalEventLogPosition Position,
        TEventBase Event,
        Runtime.GrainId EventGrainId,
        GrainEventNumber GrainEventNumber)
        : GrainEventNotification(Position, EventGrainId, GrainEventNumber);
}
