using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class TipiNormalizzazione
{
    public int IdTipoNormalizzazione { get; set; }

    public string TipoNormalizzazione { get; set; } = null!;

    public virtual ICollection<Scatole> Scatoles { get; set; } = new List<Scatole>();
}
