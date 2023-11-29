using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Tipologie
{
    public int IdTipologia { get; set; }

    public string Tipologia { get; set; }

    public DateTime? DataCreazione { get; set; }

    public int? IdOperatoreCreazione { get; set; }

    public string Note { get; set; }

    public virtual ICollection<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; } = new List<CommessaTipologiaContenitore>();

    public virtual Operatori IdOperatoreCreazioneNavigation { get; set; }

    public virtual ICollection<Scatole> Scatoles { get; } = new List<Scatole>();
}
