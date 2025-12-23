using System.Collections.Generic;
using Godot;

namespace Rework.World.Biome;

/// <summary>
/// Represents a rule for sub-biome assignment based on entity influences.
/// </summary>
public class SubBiomeRule
{
    /// <summary>
    /// The name of the sub-biome this rule applies to.
    /// </summary>
    public string SubBiomeName { get; init; }

    /// <summary>
    /// The influence thresholds required for this rule to apply.
    /// All specified influences must meet or exceed their thresholds.
    /// </summary>
    public Dictionary<string, float> InfluenceThresholds { get; init; } = new();

    /// <summary>
    /// Constructor for the SubBiomeRule class.
    /// </summary>
    /// <param name="subBiomeName">The name of the sub-biome this rule applies to.</param>
    /// <param name="influenceThresholds">The influence thresholds required for this rule to apply.</param>
    public SubBiomeRule(string subBiomeName, Dictionary<string, float> influenceThresholds)
    {
        SubBiomeName = subBiomeName;
        InfluenceThresholds = influenceThresholds;
    }
}