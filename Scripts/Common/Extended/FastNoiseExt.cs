using Godot;
using System;

namespace Common.Extended;

/// <summary>
/// Extension of the FastNoiseLite class that allows to internally normalize the noise values.
/// </summary>
[GlobalClass]
public partial class FastNoiseExt : FastNoiseLite
{
    /// <summary>
    /// Indicates whether the noise values should be normalized to the range [0, 1].
    /// If true, the noise values will be normalized based on the specified sample size.
    /// If false, the raw noise values will be returned.
    /// </summary>
    [Export]
    public bool Normalize { get; set; } = true;

    /// <summary>
    /// The size of the sample used to calculate the min and max values for normalization.
    /// This is used only if Normalize is true.
    /// The sample size is defined as a 2D vector, where the X component is the width and the Y component is the height.
    /// Bigger sample sizes will result in more accurate normalization, but will also take longer to compute.
    /// </summary>
    [Export]
    public Vector2I NormalizeSampleSize { get; private set; } = new Vector2I(100, 100);

    /// <summary>
    /// The minimum value of the noise function in the sample size.
    /// This is used to normalize the noise values to the range [0, 1].
    /// </summary>
    protected float MinValue { get; private set; } = float.MaxValue;

    /// <summary>
    /// The maximum value of the noise function in the sample size.
    /// This is used to normalize the noise values to the range [0, 1].
    /// </summary>
    protected float MaxValue { get; private set; } = float.MinValue;

    /// <summary>
    /// Gathers the min and max values of the noise function in the sample size.
    /// This is used to normalize the noise values to the range [0, 1].
    /// Called only if Normalize is true.
    /// Called only once when the first normalized value is requested.
    /// </summary>
    protected void GatherMinMax()
    {
        for (int x = 0; x < NormalizeSampleSize.X; x++)
        {
            for (int y = 0; y < NormalizeSampleSize.Y; y++)
            {
                float noiseValue = base.GetNoise2Dv(new Vector2I(x, y));
                MinValue = Math.Min(MinValue, noiseValue);
                MaxValue = Math.Max(MaxValue, noiseValue);
            }
        }
    }

    /// <summary>
    /// Normalizes the noise value to the range [0, 1].
    /// This is used only if Normalize is true.
    /// Due to the fact that the min and max values are not known exactly, only sampled from a
    /// certain area of the noise function, the result of normalization is not guaranteed to be
    /// exactly within the range [0, 1]. To fix this, the result is clamped to the range [0, 1].
    /// </summary>
    /// <param name="value">The noise value to normalize.</param>
    /// <returns>The normalized noise value in the range [0, 1].</returns>
    protected float NormalizeValue(float value)
    {
        if (MinValue == float.MaxValue && MaxValue == float.MinValue)
            GatherMinMax();
        return Mathf.Clamp((value - MinValue) / (MaxValue - MinValue), 0f, 1f);
    }

    /// <summary>
    /// Gets the noise value at the specified 1D position.
    /// This method is overridden to normalize the noise value if Normalize is true.
    /// </summary>
    /// <param name="x">The 1D position to get the noise value at.</param>
    /// <returns>The noise value at the specified position.</returns>
    public new float GetNoise1D(float x)
    {
        float noiseValue = base.GetNoise1D(x);
        return Normalize ? NormalizeValue(noiseValue) : noiseValue;
    }

    /// <summary>
    /// Gets the noise value at the specified 2D position.
    /// This method is overridden to normalize the noise value if Normalize is true.
    /// </summary>
    /// <param name="x">The X coordinate of the 2D position to get the noise value at.</param>
    /// <param name="y">The Y coordinate of the 2D position to get the noise value at.</param>
    /// <returns>The noise value at the specified position.</returns>
    public new float GetNoise2D(float x, float y)
    {
        float noiseValue = base.GetNoise2D(x, y);
        return Normalize ? NormalizeValue(noiseValue) : noiseValue;
    }

    /// <summary>
    /// Gets the noise value at the specified 2D position.
    /// This method is overridden to normalize the noise value if Normalize is true.
    /// </summary>
    /// <param name="v">The 2D position to get the noise value at.</param>
    /// <returns>The noise value at the specified position.</returns>
    public new float GetNoise2Dv(Vector2 v)
    {
        float noiseValue = base.GetNoise2Dv(v);
        return Normalize ? NormalizeValue(noiseValue) : noiseValue;
    }

    /// <summary>
    /// Gets the noise value at the specified 3D position.
    /// This method is overridden to normalize the noise value if Normalize is true.
    /// </summary>
    /// <param name="x">The X coordinate of the 3D position to get the noise value at.</param>
    /// <param name="y">The Y coordinate of the 3D position to get the noise value at.</param>
    /// <param name="z">The Z coordinate of the 3D position to get the noise value at.</param>
    /// <returns>The noise value at the specified position.</returns>
    public new float GetNoise3D(float x, float y, float z)
    {
        float noiseValue = base.GetNoise3D(x, y, z);
        return Normalize ? NormalizeValue(noiseValue) : noiseValue;
    }

    /// <summary>
    /// Gets the noise value at the specified 3D position.
    /// This method is overridden to normalize the noise value if Normalize is true.
    /// </summary>
    /// <param name="v">The 3D position to get the noise value at.</param>
    /// <returns>The noise value at the specified position.</returns>
    public new float GetNoise3Dv(Vector3 v)
    {
        float noiseValue = base.GetNoise3Dv(v);
        return Normalize ? NormalizeValue(noiseValue) : noiseValue;
    }
}