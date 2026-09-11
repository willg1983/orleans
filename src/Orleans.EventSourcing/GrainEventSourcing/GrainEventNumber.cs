using System;

namespace Orleans.EventSourcing;

/// <summary>
/// Represents the position of an event within a grain's individual event stream.
/// Event numbers are sequential and start at zero, increasing by one for each event appended.
/// </summary>
/// <param name="Value">The underlying numeric value of the event number.</param>
public readonly record struct GrainEventNumber(long Value) : IComparable<GrainEventNumber>
{
    /// <summary>
    /// Gets the starting event number representing the beginning of an event stream (before any events have been written).
    /// Pass this value to <see cref="IGrainEventStorageProvider.ReadEvents"/> to read all events from the beginning.
    /// </summary>
    public static GrainEventNumber Start { get; } = new(0L);

    /// <summary>
    /// Returns the next event number after this one.
    /// </summary>
    /// <returns>A <see cref="GrainEventNumber"/> with value incremented by one.</returns>
    public GrainEventNumber Next() => new(Value + 1);

    /// <inheritdoc />
    public int CompareTo(GrainEventNumber other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Determines whether this event number is greater than <paramref name="other"/>.
    /// </summary>
    public static bool operator >(GrainEventNumber left, GrainEventNumber right) => left.Value > right.Value;

    /// <summary>
    /// Determines whether this event number is less than <paramref name="other"/>.
    /// </summary>
    public static bool operator <(GrainEventNumber left, GrainEventNumber right) => left.Value < right.Value;

    /// <summary>
    /// Determines whether this event number is greater than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator >=(GrainEventNumber left, GrainEventNumber right) => left.Value >= right.Value;

    /// <summary>
    /// Determines whether this event number is less than or equal to <paramref name="other"/>.
    /// </summary>
    public static bool operator <=(GrainEventNumber left, GrainEventNumber right) => left.Value <= right.Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
