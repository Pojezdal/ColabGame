using System.Collections.Generic;
using Godot;

namespace Rework.World.Biome;

/// <summary>
/// Represents a sub-biome that can be assigned to tiles.
/// </summary>
public partial class SubBiome : Biome
{
    public SubBiome(string name, Dictionary<string, Common.Type.Range> properties = null) : base(name, properties)
    {
    }
}