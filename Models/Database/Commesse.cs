using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Commesse
{
    public int IdCommessa { get; set; }

    public string Commessa { get; set; }

    public DateTime? DataCreazione { get; set; }

    public int? IdOperatore { get; set; }

    public string Note { get; set; }

    public int? GiorniSla { get; set; }

    public int? IdPiattaforma { get; set; }

    public bool? Attiva { get; set; }

    public virtual ICollection<Bancali> Bancalis { get; } = new List<Bancali>();

    public virtual ICollection<CommessaTipologiaContenitore> CommessaTipologiaContenitores { get; } = new List<CommessaTipologiaContenitore>();

    public virtual Operatori IdOperatoreNavigation { get; set; }

    public virtual Piattaforme IdPiattaformaNavigation { get; set; }

    public virtual ICollection<Scatole> Scatoles { get; } = new List<Scatole>();
}
