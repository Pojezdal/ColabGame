using System.Collections.Generic;
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
    public string Name { get; private set; } = "Unnamed Biome";

    /// <summary>
    /// The properties defining the biome's characteristics.
    /// </summary>
    public Dictionary<string, Property> Properties { get; init; } = new();

    /// <summary>
    /// Constructor for the Biome class.
    /// </summary>
    /// <param name="name">The name of the biome.</param>
    /// <param name="properties">The properties defining the biome's characteristics.</param>
    public Biome(string name, Dictionary<string, Property> properties = null)
    {
        Name = name;
        if (properties != null)
            Properties = properties;
    }
}