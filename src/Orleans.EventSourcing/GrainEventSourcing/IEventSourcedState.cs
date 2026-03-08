using System.Collections.Generic;
using System.Threading;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents an event-sourced read model that is injected into an event-sourced grain's constructor.
/// </summary>
/// <typeparam name="TReadModel">The type of the read model maintained by this state.</typeparam>
/// <remarks>
/// <para>
/// <see cref="IEventSourcedState{TReadModel}"/> is the primary way for grains to access and update
/// their event-sourced state.  The framework loads historical events from
/// <see cref="IGrainEventStorageProvider"/> on activation and applies them to rebuild the read model.
/// </para>
/// <para>
/// When events are written via the grain's pending-event writer, all injected
/// <see cref="IEventSourcedState{TReadModel}"/> instances are updated synchronously so that
/// re-entrant operations observe an up-to-date view.  The observable stream of state changes
/// (exposed through <see cref="IAsyncEnumerable{T}"/>) yields after the write has been durably
/// persisted to prevent phantom state from escaping non-reentrant grains.
/// </para>
/// <para>
/// A grain may inject multiple <see cref="IEventSourcedState{TReadModel}"/> instances with
/// different read-model types to separate concerns (e.g. command-validation state,
/// audit history, status projections).
/// </para>
/// </remarks>
public interface IEventSourcedState<TReadModel> : IAsyncEnumerable<TReadModel>
{
    /// <summary>
    /// Gets the current read model, reflecting all events that have been applied so far.
    /// </summary>
    TReadModel Value { get; }

    /// <summary>
    /// Gets the event number of the most recent event applied to this state.
    /// </summary>
    /// <remarks>
    /// Before any events have been applied this is <see cref="GrainEventNumber.Initial"/>.
    /// </remarks>
    GrainEventNumber EventNumber { get; }

    /// <summary>
    /// Returns an <see cref="IAsyncEnumerator{T}"/> that yields the read model each time the
    /// durable state changes (i.e., after a successful write to storage).
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to stop the enumeration.</param>
    new IAsyncEnumerator<TReadModel> GetAsyncEnumerator(CancellationToken cancellationToken = default);
}
