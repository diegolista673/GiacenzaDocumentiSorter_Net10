using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class BancaliDispacci
{
    public int IdAssociazione { get; set; }

    public int IdBancale { get; set; }

    public string Operatore { get; set; } = null!;

    public DateTime DataAssociazione { get; set; }

    public int IdCentro { get; set; }

    public string Bancale { get; set; } = null!;

    public string Dispaccio { get; set; } = null!;

    public string Centro { get; set; } = null!;

    public virtual Bancali IdBancaleNavigation { get; set; } = null!;
}
