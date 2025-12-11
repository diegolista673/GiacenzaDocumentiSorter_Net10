using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Piattaforme
{
    public int IdPiattaforma { get; set; }

    public string? Piattaforma { get; set; }

    public DateTime? DataCreazione { get; set; }

    public int? IdOperatoreCreazione { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Commesse> Commesses { get; set; } = new List<Commesse>();
}
