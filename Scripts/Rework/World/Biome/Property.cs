using Godot;

namespace Rework.World.Biome;

/// <summary>
/// Represents a property of a biome, defined by a range of possible values.
/// The boundaries can be the same, indicating a fixed value.
/// </summary>
public class Property
{
    /// <summary>
    /// The minimum value of the property.
    /// </summary>
    public float Min { get; private set; }

    /// <summary>
    /// The maximum value of the property.
    /// </summary>
    public float Max { get; private set; }

    /// <summary>
    /// Constructor for the Property class.
    /// </summary>
    /// <param name="min">The minimum value of the property.</param>
    /// <param name="max">The maximum value of the property.</param>
    public Property(float min, float max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Constructor for the Property class with a fixed value.
    /// </summary>
    /// <param name="value">The fixed value of the property.</param>
    public Property(float value) : this(value, value) { }

    /// <summary>
    /// Gets a value within the property's range based on a weight (0 to 1).
    /// Using linear interpolation.
    /// </summary>
    /// <param name="weight">The weight (0 to 1) to determine the value.</param>
    /// <returns>A value within the property's range.</returns>
    public float FromWeight(float weight) => Mathf.Lerp(Min, Max, weight);

    /// <summary>
    /// Gets a random value within the property's range using a uniform distribution.
    /// </summary>
    /// <returns>A random value within the property's range.</returns>
    public float FromUniform() => (Min != Max) ? Utils.RNG.Instance.Float(Min, Max) : Min;

    /// <summary>
    /// Gets a random value within the property's range using a normal (Gaussian) distribution.
    /// The mean is the midpoint of the range, and the standard deviation is one-sixth of the range.
    /// </summary>
    /// <returns>A random value within the property's range.</returns>
    public float FromNormal() => (Min != Max) ? Utils.RNG.Instance.FloatGaussian((Min + Max) / 2f, (Max - Min) / 6f) : Min;
}