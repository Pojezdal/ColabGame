using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils;

/// <summary>
/// Random number generator utility class.
/// Provides methods for generating random integers, floats, vectors, and making random choices.
/// Uses Godot's RandomNumberGenerator internally.
/// Can create multiple independent RNG instances if needed or use the singleton instance.
/// </summary>
public class RNG
{
    /// <summary>
    /// Singleton instance of the RNG class, that can be used globally.
    /// </summary>
    public static readonly RNG Instance = new RNG();

    /// <summary>
    /// Internal Godot RandomNumberGenerator instance.
    /// </summary>
    private RandomNumberGenerator _gdRng = new RandomNumberGenerator();

    /// <summary>
    /// Initializes a new instance of the RNG class and randomizes the internal state.
    /// </summary>
    /// <param name="seed">Optional seed value to initialize the RNG. If null, the RNG is randomized.</param>
    public RNG(ulong? seed = null)
    {
        if (seed.HasValue)
            Seed(seed.Value);
        else
            _gdRng.Randomize();
    }

    /// <summary>
    /// Seeds the random number generator with the specified seed.
    /// </summary>
    /// <param name="seed">The seed value.</param>
    public void Seed(ulong seed)
    {
        _gdRng.Seed = seed;
    }

    /// <summary>
    /// Generates a random integer between min (inclusive) and max (inclusive).
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns>A random integer between min and max.</returns>
    public int Int(int min = 0, int max = int.MaxValue)
    {
        return _gdRng.RandiRange(min, max);
    }

    /// <summary>
    /// Generates a random float between min (inclusive) and max (inclusive).
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns>A random float between min and max.</returns>
    public float Float(float min = 0f, float max = 1f)
    {
        return _gdRng.RandfRange(min, max);
    }

    /// <summary>
    /// Generates a random float within the specified range (inclusive).
    /// </summary>
    /// <param name="range">The range specifying the minimum and maximum values.</param>
    /// <returns>A random float within the specified range.</returns>
    public float Float(Common.Type.Range range) => Float(range.Min, range.Max);

    /// <summary>
    /// Generates a random float following a Gaussian (normal) distribution with the specified mean and standard deviation.
    /// </summary>
    /// <param name="mean">The mean value of the distribution.</param>
    /// <param name="deviation">The standard deviation of the distribution.</param>
    /// <returns>A random float following a Gaussian distribution.</returns>
    public float FloatGaussian(float mean = 0f, float deviation = 1f)
    {
        return _gdRng.Randfn(mean, deviation);
    }

    /// <summary>
    /// Generates a random float following a Gaussian (normal) distribution within the specified range.
    /// The mean is set to the midpoint of the range, and the standard deviation is set so that ~99.7% of values fall within the range.
    /// </summary>
    /// <param name="range">The range specifying the minimum and maximum values.</param>
    public float FloatGaussian(Common.Type.Range range) => FloatGaussian((range.Min + range.Max) / 2f, (range.Max - range.Min) / 6f);

    /// <summary>
    /// Returns true with the specified probability.
    /// </summary>
    /// <param name="probability">The probability of returning true (between 0 and 1).</param>
    /// <returns>True with the specified probability, otherwise false.</returns>
    public bool Chance(float probability)
    {
        return _gdRng.Randf() < probability;
    }

    /// <summary>
    /// Generates a random Vector2I with each component between the specified min (inclusive) and max (inclusive) values.
    /// </summary>
    /// <param name="minX">The minimum X value (inclusive).</param>
    /// <param name="maxX">The maximum X value (inclusive).</param>
    /// <param name="minY">The minimum Y value (inclusive).</param>
    /// <param name="maxY">The maximum Y value (inclusive).</param>
    /// <returns>A random Vector2I.</returns>
    public Vector2I Vector2I(int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue)
    {
        return new Vector2I(Int(minX, maxX), Int(minY, maxY));
    }

    /// <summary>
    /// Generates a random Vector2 with each component between the specified min (inclusive) and max (inclusive) values.
    /// </summary>
    /// <param name="minX">The minimum X value (inclusive).</param>
    /// <param name="maxX">The maximum X value (inclusive).</param>
    /// <param name="minY">The minimum Y value (inclusive).</param>
    /// <param name="maxY">The maximum Y value (inclusive).</param>
    /// <returns>A random Vector2.</returns>
    public Vector2 Vector2(float minX = 0f, float maxX = 1f, float minY = 0f, float maxY = 1f)
    {
        return new Vector2(Float(minX, maxX), Float(minY, maxY));
    }

    /// <summary>
    /// Generates a random Vector3I with each component between the specified min (inclusive) and max (inclusive) values.
    /// </summary>
    /// <param name="minX">The minimum X value (inclusive).</param>
    /// <param name="maxX">The maximum X value (inclusive).</param>
    /// <param name="minY">The minimum Y value (inclusive).</param>
    /// <param name="maxY">The maximum Y value (inclusive).</param>
    /// <param name="minZ">The minimum Z value (inclusive).</param>
    /// <param name="maxZ">The maximum Z value (inclusive).</param>
    /// <returns>A random Vector3I.</returns>
    public Vector3I Vector3I(int minX = 0, int maxX = int.MaxValue, int minY = 0, int maxY = int.MaxValue, int minZ = 0, int maxZ = int.MaxValue)
    {
        return new Vector3I(Int(minX, maxX), Int(minY, maxY), Int(minZ, maxZ));
    }

    /// <summary>
    /// Generates a random Vector3 with each component between the specified min (inclusive) and max (inclusive) values.
    /// </summary>
    /// <param name="minX">The minimum X value (inclusive).</param>
    /// <param name="maxX">The maximum X value (inclusive).</param>
    /// <param name="minY">The minimum Y value (inclusive).</param>
    /// <param name="maxY">The maximum Y value (inclusive).</param>
    /// <param name="minZ">The minimum Z value (inclusive).</param>
    /// <param name="maxZ">The maximum Z value (inclusive).</param>
    /// <returns>A random Vector3.</returns>
    public Vector3 Vector3(float minX = 0f, float maxX = 1f, float minY = 0f, float maxY = 1f, float minZ = 0f, float maxZ = 1f)
    {
        return new Vector3(Float(minX, maxX), Float(minY, maxY), Float(minZ, maxZ));
    }

    /// <summary>
    /// Selects a random element from the given array and outputs its index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to select from.</param>
    /// <param name="index">The index of the selected element.</param>
    /// <returns>A random element from the array.</returns>
    public T Choice<T>(IEnumerable<T> array, out int index)
    {
        if (array == null || !array.Any())
            throw new ArgumentException("Array cannot be empty.");
        index = Int(0, array.Count() - 1);
        return array.ElementAt(index);
    }

    /// <summary>
    /// Selects a random element from the given array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to select from.</param>
    /// <returns>A random element from the array.</returns>
    public T Choice<T>(IEnumerable<T> array)
    {
        return Choice(array, out int index);
    }

    /// <summary>
    /// Selects multiple random elements from the given array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to select from.</param>
    /// <param name="count">The number of elements to select.</param>
    /// <param name="unique">Whether the selected elements should be unique.</param>
    /// <returns>A list of random elements from the array.</returns>
    public List<T> Choices<T>(IEnumerable<T> array, int count, bool unique = false)
    {
        if (array == null || !array.Any())
            throw new ArgumentException("Array cannot be empty.");
        if (unique && count > array.Count())
            throw new ArgumentException("Count cannot be greater than array size when unique is true.");

        var result = new List<T>();
        var available = unique ? array.ToList() : null;

        for (int i = 0; i < count; i++)
        {
            T choice;
            if (unique)
            {
                int index = Int(0, available.Count - 1);
                choice = available[index];
                available.RemoveAt(index);
            }
            else
            {
                choice = Choice(array);
            }
            result.Add(choice);
        }

        return result;
    }

    /// <summary>
    /// Generates a random Vector2I within a circle of the specified radius and optional center.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="centerX">The X coordinate of the circle's center.</param>
    /// <param name="centerY">The Y coordinate of the circle's center.</param>
    /// <param name="minRadius">The minimum radius from the center.</param>
    /// <returns>A random Vector2I within the circle.</returns>
    public Vector2I Vector2ICircle(float radius, int centerX = 0, int centerY = 0, float minRadius = 0f)
    {
        float angle = Float(0f, Mathf.Pi * 2);
        float r = Mathf.Sqrt(Float(minRadius * minRadius, radius * radius));
        return new Vector2I(Mathf.FloorToInt(centerX + r * Mathf.Cos(angle)), Mathf.FloorToInt(centerY + r * Mathf.Sin(angle)));
    }

    /// <summary>
    /// Generates a random Vector2 within a circle of the specified radius and optional center.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="centerX">The X coordinate of the circle's center.</param>
    /// <param name="centerY">The Y coordinate of the circle's center.</param>
    /// <param name="minRadius">The minimum radius from the center.</param>
    /// <returns>A random Vector2 within the circle.</returns>
    public Vector2 Vector2Circle(float radius, float centerX = 0f, float centerY = 0f, float minRadius = 0f)
    {
        float angle = Float(0f, Mathf.Pi * 2);
        float r = Mathf.Sqrt(Float(minRadius * minRadius, radius * radius));
        return new Vector2(centerX + r * Mathf.Cos(angle), centerY + r * Mathf.Sin(angle));
    }
}