using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class SettingsCommesse
{
    public int IdSettingsCommesse { get; set; }

    public int IdCommessa { get; set; }

    public int IdCentro { get; set; }

    public bool FlagAccettazione { get; set; }

    public string Centro { get; set; }

    public virtual CentriLav IdCentroNavigation { get; set; }

    public virtual Commesse IdCommessaNavigation { get; set; }
}
