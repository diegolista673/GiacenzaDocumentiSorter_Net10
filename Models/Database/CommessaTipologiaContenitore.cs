using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class CommessaTipologiaContenitore
{
    public int IdRiepilogo { get; set; }

    public int? IdCommessa { get; set; }

    public string DescCommessa { get; set; }

    public int? IdTipologia { get; set; }

    public string DescTipologia { get; set; }

    public int? IdContenitore { get; set; }

    public string DescContenitore { get; set; }

    public int? Quantita { get; set; }
}
