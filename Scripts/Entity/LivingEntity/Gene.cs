using Godot;
using System.Collections.Generic;

namespace Entity.LivingEntity;

/// <summary>
/// Represents a gene of a living entity.
/// These define inherited characteristics that can affect traits and behaviors,
/// but are immutable once set.
/// </summary>
public partial class Gene : RefCounted
{
    /// <summary>
    /// The name of the gene.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The value of the gene.
    /// </summary>
    public float Value { get; init; } = 0.0f;

    /// <summary>
    /// Initializes a new instance of the <see cref="Gene"/> class.
    /// </summary>
    /// <param name="name">The name of the gene.</param>
    public Gene(string name)
    {
        Name = name;
    }
}