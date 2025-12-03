using Godot;
using System;
using System.Collections.Generic;

namespace Entity.Animal;

/// <summary>
/// Represents a rabbit.
/// </summary>
public partial class Rabbit : Animal
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Rabbit"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the rabbit.</param>
    /// <returns>A new instance of the <see cref="Rabbit"/> class.</returns>
    public Rabbit(string id) : base(id,
        new List<Property>
        {
            new Property("Health", 100.0f),
            new Property("Hunger", 100.0f),
            new Property("Age", 0.0f),
        },
        new List<Gene>
        {
            new Gene("Size") { Value = 1.0f },
            new Gene("Speed") { Value = 1.0f },
            new Gene("LifeSpan") { Value = 5.0f },
        },
        new List<Trait>
        {
            new Trait("Size", (properties, genes) =>
            {
                return Mathf.Min(1.0f, 0.2f + properties["Age"].Value / genes["LifeSpan"].Value) * genes["Size"].Value;
            }),
            new Trait("MovementSpeed", (properties, genes) =>
            {
                return properties["Age"].Value / genes["LifeSpan"].Value * genes["Speed"].Value * 10.0f;
            }),
        })
    {
    }

    /// <summary>
    /// Creates and returns a new EntityNode associated with this rabbit.
    /// </summary>
    /// <returns>A new EntityNode instance associated with this rabbit.</returns>
    public override EntityNode CreateNode()
    {
        throw new NotImplementedException();
    }
}