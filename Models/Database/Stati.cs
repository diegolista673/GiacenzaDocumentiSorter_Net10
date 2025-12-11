using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Stati
{
    public int IdStato { get; set; }

    public string? Stato { get; set; }

    public virtual ICollection<Scatole> Scatoles { get; set; } = new List<Scatole>();
}
