using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class CentriLav
{
    public int IdCentroLavorazione { get; set; }

    public string CentroLavDesc { get; set; }

    public string Sigla { get; set; }

    public virtual ICollection<Bancali> Bancalis { get; } = new List<Bancali>();

    public virtual ICollection<Operatori> Operatoris { get; } = new List<Operatori>();

    public virtual ICollection<Scatole> ScatoleIdCentroNormalizzazioneNavigations { get; } = new List<Scatole>();

    public virtual ICollection<Scatole> ScatoleIdCentroSorterizzazioneNavigations { get; } = new List<Scatole>();

    public virtual ICollection<SettingsCommesse> SettingsCommesses { get; } = new List<SettingsCommesse>();
}
