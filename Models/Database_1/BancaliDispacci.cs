using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class BancaliDispacci
{
    public int IdAssociazione { get; set; }

    public int IdBancale { get; set; }

    public string Operatore { get; set; }

    public DateTime DataAssociazione { get; set; }

    public int IdCentro { get; set; }

    public string Bancale { get; set; }

    public string Dispaccio { get; set; }

    public string Centro { get; set; }

    public virtual Bancali IdBancaleNavigation { get; set; }
}
