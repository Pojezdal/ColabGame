using Godot;
using System;
using System.Collections.Generic;

namespace Entity;

public partial class Plant : Entity
{
    public Plant(string id, List<Property> properties) : base(id, new() { new("reproduction_rate", 30), new("reproduction_radius", 30) })
    {

    }
}