using System;
using System.Collections.Generic;
using System.Threading;
using Orleans.Runtime;

namespace Orleans.EventSourcing;

/// <summary>
/// Provides a mechanism to subscribe to events emitted by grains, enabling eventually-consistent
/// projections and side-effects.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IGrainEventProvider"/> delivers events in global log order, ensuring that
/// projections observe events in the same order in which they were persisted.
/// </para>
/// <para>
/// Subscribers receive a stream of <see cref="EventStreamUpdate"/> discriminated-union values that
/// include payloads (<see cref="EventStreamUpdate.GrainEvent{TEventBase}"/>), catchup/lag
/// notifications (<see cref="EventStreamUpdate.CaughtUp"/>, <see cref="EventStreamUpdate.FallenBehind"/>),
/// and periodic checkpoints (<see cref="EventStreamUpdate.Checkpoint"/>) that enable durable
/// cursor tracking.
/// </para>
/// </remarks>
public interface IGrainEventProvider
{
    /// <summary>
    /// Subscribes to a filtered stream of grain events, starting from the given global position.
    /// </summary>
    /// <typeparam name="TEventBase">The base type of the event payload to receive.</typeparam>
    /// <param name="subscriber">
    /// The <see cref="GrainId"/> of the subscribing grain, used for observability and logging.
    /// </param>
    /// <param name="startingPosition">
    /// The <see cref="GlobalEventLogPosition"/> from which to start the subscription.
    /// Use <see cref="GlobalEventLogPosition.Start"/> to replay all available events from the beginning.
    /// </param>
    /// <param name="eventFilter">
    /// An array of .NET event types to include in the subscription.
    /// Only events whose deserialized type is assignable to one of these types will be delivered.
    /// Pass an empty array to receive all events.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel the subscription.</param>
    /// <returns>
    /// An asynchronous sequence of <see cref="EventStreamUpdate"/> values representing events,
    /// checkpoints, and lag notifications in global log order.
    /// </returns>
    IAsyncEnumerable<EventStreamUpdate> SubscribeToGrainEvents<TEventBase>(
        GrainId subscriber,
        GlobalEventLogPosition startingPosition,
        Type[] eventFilter,
        CancellationToken cancellationToken)
        where TEventBase : notnull;

    /// <summary>
    /// Subscribes to all events emitted by grains of type <typeparamref name="TGrain"/>,
    /// starting from the given global position.
    /// </summary>
    /// <typeparam name="TGrain">The grain type whose events to subscribe to.</typeparam>
    /// <typeparam name="TEventBase">The base type of the event payload to receive.</typeparam>
    /// <param name="subscriber">
    /// The <see cref="GrainId"/> of the subscribing grain, used for observability and logging.
    /// </param>
    /// <param name="startingPosition">
    /// The <see cref="GlobalEventLogPosition"/> from which to start the subscription.
    /// Use <see cref="GlobalEventLogPosition.Start"/> to replay all available events from the beginning.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel the subscription.</param>
    /// <returns>
    /// An asynchronous sequence of <see cref="EventStreamUpdate"/> values representing events,
    /// checkpoints, and lag notifications in global log order.
    /// </returns>
    IAsyncEnumerable<EventStreamUpdate> SubscribeToGrainEvents<TGrain, TEventBase>(
        GrainId subscriber,
        GlobalEventLogPosition startingPosition,
        CancellationToken cancellationToken)
        where TGrain : IGrain
        where TEventBase : notnull;
}
