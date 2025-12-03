using Godot;
using System;
using System.Collections.Generic;

namespace Entity.Animal;

/// <summary>
/// Represents a trait of a living entity.
/// These represent observable characteristics influenced by genes and properties.
/// </summary>
public partial class Trait : RefCounted
{
    /// <summary>
    /// The name of the trait.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// A function to calculate the value of the trait based on properties and genes.
    /// </summary>
    public Func<Dictionary<string, Property>, Dictionary<string, Gene>, float> CalculateValue { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Trait"/> class.
    /// </summary>
    /// <param name="name">The name of the trait.</param>
    /// <param name="calculateValue">A function to calculate the value of the trait based on properties and genes.</param>
    public Trait(string name, Func<Dictionary<string, Property>, Dictionary<string, Gene>, float> calculateValue)
    {
        Name = name;
        CalculateValue = calculateValue;
    }
}