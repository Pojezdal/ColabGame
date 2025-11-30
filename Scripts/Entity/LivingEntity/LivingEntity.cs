using Godot;
using System;
using System.Collections.Generic;

namespace Entity.LivingEntity;

/// <summary>
/// Base class for all living entities in the world.
/// These can interact with their environment and other entities in more complex ways and can reproduce.
/// </summary>
public partial class LivingEntity : Entity
{
    /// <summary>
    /// The genes of this living entity.
    /// These define inherited characteristics that can affect traits and behaviors.
    /// </summary>
    public Dictionary<string, Gene> Genes { get; init; } = new();

    /// <summary>
    /// The traits of this living entity.
    /// These represent observable characteristics influenced by genes and properties.
    /// </summary>
    private Dictionary<string, Trait> Traits { get; init; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LivingEntity"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the living entity.</param>
    /// <param name="properties">The initial properties of the living entity.</param>
    /// <param name="genes">The initial genes of the living entity.</param>
    /// <param name="traits">The initial traits of the living entity.</param>
    public LivingEntity(string id, List<Property> properties, List<Gene> genes, List<Trait> traits) : base(id, properties)
    {
        foreach (var gene in genes)
        {
            Genes[gene.Name] = gene;
        }

        foreach (var trait in traits)
        {
            Traits[trait.Name] = trait;
        }
    }

    /// <summary>
    /// Calculates the value of a specified trait based on the current properties and genes.
    /// </summary>
    /// <param name="traitName">The name of the trait to calculate the value for.</param>
    /// <returns>The calculated value of the trait.</returns>
    public float TraitValue(string traitName)
    {
        if (Traits.TryGetValue(traitName, out Trait value))
        {
            return value.CalculateValue(Properties, Genes);
        }
        else
        {
            GD.PrintErr($"Trait '{traitName}' not found in LivingEntity '{Id}'.");
            return 0.0f;
        }
    }
}