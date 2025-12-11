using Godot;

namespace Rework.World.Biome;

/// <summary>
/// Represents a sub-biome that can be assigned to tiles.
/// </summary>
public partial class SubBiome : Resource
{
    /// <summary>
    /// The name of the sub-biome.
    /// </summary>
    [Export]
    public string Name { get; private set; } = "DefaultSubBiome";
}