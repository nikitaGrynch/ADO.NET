using System;
using System.Collections.Generic;

namespace ADO_NET.Scaffolded;

public partial class Sale
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid ManagerId { get; set; }

    public int Cnt { get; set; }

    public DateTime SaleDt { get; set; }

    public DateTime? DeleteDt { get; set; }
}
