﻿using System;
using System.Collections.Generic;

namespace ADO_NET.Scaffolded;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public float Price { get; set; }

    public DateTime? DeleteDt { get; set; }
}
