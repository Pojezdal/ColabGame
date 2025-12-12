using Godot;

namespace Common.Type;

/// <summary>
/// Represents a range of float values with minimum and maximum bounds.
/// Supports several utility methods for range checking and interpolation.
/// </summary>
public struct Range
{
    /// <summary>
    /// The minimum value of the range.
    /// </summary>
    public float Min { get; init; }

    /// <summary>
    /// The maximum value of the range.
    /// </summary>
    public float Max { get; init; }

    /// <summary>
    /// Constructor for the Range struct.
    /// </summary>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    public Range(float min = float.MinValue, float max = float.MaxValue)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Constructor for the Range struct with a single value.
    /// </summary>
    /// <param name="value">The value to set both minimum and maximum to.</param
    public Range(float value) : this(value, value) { }

    /// <summary>
    /// Checks if a given value is within the range [Min, Max].
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is within the range, otherwise false.</returns>
    public readonly bool Contains(float value) => value >= Min && value <= Max;

    /// <summary>
    /// Linearly interpolates between the minimum and maximum values based on the given weight.
    /// </summary>
    /// <param name="weight">The interpolation weight, typically between 0 and 1.</param>
    /// <returns>The interpolated value.</returns>
    public readonly float Lerp(float weight) => Mathf.Lerp(Min, Max, weight);

    /// <summary>
    /// Calculates the interpolation weight of a given value within the range [Min, Max].
    /// </summary>
    /// <param name="value">The value to calculate the interpolation weight for.</param>
    /// <returns>The interpolation weight, typically between 0 and 1.</returns>
    public readonly float InverseLerp(float value) => Mathf.InverseLerp(Min, Max, value);

}