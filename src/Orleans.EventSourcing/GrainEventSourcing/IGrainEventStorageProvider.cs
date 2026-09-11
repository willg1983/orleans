using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.EventSourcing;

/// <summary>
/// Defines the interface for a storage provider that supports reading, writing, and deleting
/// events in a grain's dedicated event stream.
/// </summary>
/// <remarks>
/// <para>
/// Implementations use optimistic concurrency control on writes: a write only succeeds if the
/// current event number in storage matches <c>expectedEventNumber</c>.  A mismatch causes an
/// <see cref="InconsistentStateException"/> to be thrown, allowing the caller to detect and
/// resolve concurrent write conflicts.
/// </para>
/// <para>
/// Each grain owns a single dedicated event stream identified by its <see cref="GrainId"/>.
/// </para>
/// </remarks>
public interface IGrainEventStorageProvider
{
    /// <summary>
    /// Reads all events for the specified grain starting after the given event number, in ascending order.
    /// </summary>
    /// <remarks>
    /// The <paramref name="exclusiveFrom"/> parameter is used as the exclusive lower bound for reads.
    /// Previously truncated events may cause the first event returned to have an event number greater
    /// than <c>exclusiveFrom + 1</c>.
    /// </remarks>
    /// <param name="grainId">The identity of the grain whose events should be read.</param>
    /// <param name="exclusiveFrom">
    /// The event number from which to start reading, exclusive.
    /// Pass <see cref="GrainEventNumber.Start"/> to read from the very beginning of the stream.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel the read operation.</param>
    /// <returns>An asynchronous sequence of <see cref="EventRead"/> values in ascending event-number order.</returns>
    IAsyncEnumerable<EventRead> ReadEvents(GrainId grainId, GrainEventNumber exclusiveFrom, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically writes one or more events to the grain's event stream using optimistic concurrency control.
    /// </summary>
    /// <remarks>
    /// The write succeeds only when the current highest event number in storage equals
    /// <paramref name="expectedEventNumber"/>.  If another writer has appended events since the caller
    /// last read, the provider throws <see cref="InconsistentStateException"/>.
    /// </remarks>
    /// <param name="grainId">The identity of the grain whose events should be written.</param>
    /// <param name="expectedEventNumber">
    /// The event number the caller believes to be the current tail of the stream.
    /// For a brand-new stream use <see cref="GrainEventNumber.Start"/>.
    /// </param>
    /// <param name="events">The events to append.  All events are written atomically as a single batch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the write operation.</param>
    /// <returns>
    /// The <see cref="GlobalEventLogPosition"/> of the last written event, which can be used as a
    /// starting position for projection subscriptions.
    /// </returns>
    /// <exception cref="InconsistentStateException">
    /// Thrown when <paramref name="expectedEventNumber"/> does not match the current tail of the stream in storage.
    /// </exception>
    ValueTask<GlobalEventLogPosition> WriteEvents(
        GrainId grainId,
        GrainEventNumber expectedEventNumber,
        ReadOnlyMemory<EventWrite> events,
        CancellationToken cancellationToken);

    /// <summary>
    /// Removes all events up to and including the specified event number from the grain's event stream.
    /// </summary>
    /// <remarks>
    /// This operation is intended for log compaction scenarios where old events are no longer needed
    /// because their effect has been captured in a snapshot.
    /// </remarks>
    /// <param name="grainId">The identity of the grain whose events should be deleted.</param>
    /// <param name="inclusiveUpTo">
    /// The event number up to which all events will be removed (inclusive).
    /// Must not exceed the current highest event number in storage.
    /// </param>
    /// <param name="cancellationToken">A token that can be used to cancel the delete operation.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the events have been deleted.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when <paramref name="inclusiveUpTo"/> is greater than the current highest event number in storage.
    /// </exception>
    ValueTask DeleteEvents(GrainId grainId, GrainEventNumber inclusiveUpTo, CancellationToken cancellationToken);
}
