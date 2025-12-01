using Godot;
using System;
using System.Collections.Generic;

namespace Entity;

public partial class Carrot : Plant
{
    public Carrot(string id) : base(id, new() { new("nutrients", 30.0f) })
    {
    }
}
