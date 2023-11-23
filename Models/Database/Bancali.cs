using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Bancali
{
    public int IdBancale { get; set; }

    public int IdCommessa { get; set; }

    public int IdCentroArrivo { get; set; }

    public string Bancale { get; set; }

    public DateTime DataAccettazioneBancale { get; set; }

    public string Note { get; set; }

    public int IdOperatoreAccettazione { get; set; }

    public string OperatoreAccettazione { get; set; }

    public int? IdCentroDestinazione { get; set; }

    public DateTime? DataInvioAltroCentro { get; set; }

    public int? IdOperatoreInvioAltroCentro { get; set; }

    public string OperatoreInvioAltroCentro { get; set; }

    public DateTime? DataSorter { get; set; }

    public int? Progressivo { get; set; }

    public virtual ICollection<BancaliDispacci> BancaliDispaccis { get; } = new List<BancaliDispacci>();

    public virtual CentriLav IdCentroArrivoNavigation { get; set; }

    public virtual Commesse IdCommessaNavigation { get; set; }

    public virtual Operatori IdOperatoreAccettazioneNavigation { get; set; }
}
