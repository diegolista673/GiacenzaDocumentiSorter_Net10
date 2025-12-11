using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Contenitori
{
    public int IdContenitore { get; set; }

    public string? Contenitore { get; set; }

    public DateTime? DataCreazione { get; set; }

    public int? TotaleDocumenti { get; set; }

    public int? IdOperatoreCreazione { get; set; }

    public virtual ICollection<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; set; } = new List<CommessaTipologiaContenitore>();

    public virtual Operatori? IdOperatoreCreazioneNavigation { get; set; }

    public virtual ICollection<Scatole> Scatoles { get; set; } = new List<Scatole>();
}
