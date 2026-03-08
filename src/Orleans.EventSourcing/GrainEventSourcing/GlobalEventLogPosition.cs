using System;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents a position in the global event log that spans all grain event streams.
/// This can be used to establish a total ordering of events across all grains and to resume
/// projection subscriptions from a known point in the log.
/// </summary>
/// <param name="Value">The underlying numeric value representing the position in the global log.</param>
public readonly record struct GlobalEventLogPosition(long Value) : IComparable<GlobalEventLogPosition>
{
    /// <summary>
    /// Gets the position representing the very beginning of the global event log (before any events have been written).
    /// </summary>
    public static GlobalEventLogPosition Start { get; } = new(0L);

    /// <inheritdoc />
    public int CompareTo(GlobalEventLogPosition other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Determines whether this position is greater than <paramref name="other"/>.
    /// </summary>
    public static bool operator >(GlobalEventLogPosition left, GlobalEventLogPosition right) => left.Value > right.Value;

    /// <summary>
    /// Determines whether this position is less than <paramref name="other"/>.
    /// </summary>
    public static bool operator <(GlobalEventLogPosition left, GlobalEventLogPosition right) => left.Value < right.Value;

    /// <summary>
    /// Determines whether this position is greater than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator >=(GlobalEventLogPosition left, GlobalEventLogPosition right) => left.Value >= right.Value;

    /// <summary>
    /// Determines whether this position is less than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator <=(GlobalEventLogPosition left, GlobalEventLogPosition right) => left.Value <= right.Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
