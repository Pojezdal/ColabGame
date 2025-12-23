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
    public Dictionary<string, Common.Type.Range> Properties { get; init; }

    /// <summary>
    /// The rules for sub-biome assignment based on entity influences.
    /// They are evaluated in order, so the order essentially defines priority.
    /// </summary>
    public List<SubBiomeRule> Rules { get; init; }

    /// <summary>
    /// Constructor for the Biome class.
    /// </summary>
    /// <param name="name">The name of the biome.</param>
    /// <param name="properties">The properties defining the biome's characteristics.</param>
    /// <param name="rules">The rules for sub-biome assignment based on entity influences.</param>
    public Biome(string name, Dictionary<string, Common.Type.Range> properties = null, List<SubBiomeRule> rules = null)
    {
        Name = name;
        Properties = properties ?? new();
        Rules = rules ?? new();
    }
}