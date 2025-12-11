using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Operatori
{
    public int IdOperatore { get; set; }

    public string? Operatore { get; set; }

    public int? IdCentroLav { get; set; }

    public string? Ruolo { get; set; }

    public string? Azienda { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Bancali> Bancalis { get; set; } = new List<Bancali>();

    public virtual ICollection<Commesse> Commesses { get; set; } = new List<Commesse>();

    public virtual ICollection<Contenitori> Contenitoris { get; set; } = new List<Contenitori>();

    public virtual CentriLav? IdCentroLavNavigation { get; set; }

    public virtual ICollection<Tipologie> Tipologies { get; set; } = new List<Tipologie>();
}
