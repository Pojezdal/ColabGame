using Godot;

namespace Rework.World.Biome;

/// <summary>
/// Represents a biome that can be assigned to tiles.
/// </summary>
public partial class Biome : Resource
{
    /// <summary>
    /// The name of the biome.
    /// </summary>
    [Export]
    public string Name { get; private set; } = "DefaultBiome";

    /// <summary>
    /// Constructor for the Biome class.
    /// </summary>
    /// <param name="name">The name of the biome.</param>
    public Biome(string name)
    {
        Name = name;
    }
}