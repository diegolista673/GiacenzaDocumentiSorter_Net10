using System;
using System.Collections.Generic;

namespace GiacenzaSorterRm.Models.Database;

public partial class Scatole
{
    public int IdScatola { get; set; }

    public string Scatola { get; set; }

    public DateTime DataNormalizzazione { get; set; }

    public string OperatoreNormalizzazione { get; set; }

    public DateTime? DataSorter { get; set; }

    public string OperatoreSorter { get; set; }

    public string Note { get; set; }

    public int IdContenitore { get; set; }

    public int IdCommessa { get; set; }

    public int IdTipologia { get; set; }

    public int IdStato { get; set; }

    public int IdTipoNormalizzazione { get; set; }

    public int IdCentroNormalizzazione { get; set; }

    public int? IdCentroSorterizzazione { get; set; }

    public DateTime? DataCambioGiacenza { get; set; }

    public int? IdCentroGiacenza { get; set; }

    public string OperatoreCambioGiacenza { get; set; }

    public string NoteCambioGiacenza { get; set; }

    public string OperatoreMacero { get; set; }

    public DateTime? DataMacero { get; set; }

    public virtual CentriLav IdCentroGiacenzaNavigation { get; set; }

    public virtual CentriLav IdCentroNormalizzazioneNavigation { get; set; }

    public virtual CentriLav IdCentroSorterizzazioneNavigation { get; set; }

    public virtual Commesse IdCommessaNavigation { get; set; }

    public virtual Contenitori IdContenitoreNavigation { get; set; }

    public virtual Stati IdStatoNavigation { get; set; }

    public virtual TipiNormalizzazione IdTipoNormalizzazioneNavigation { get; set; }

    public virtual Tipologie IdTipologiaNavigation { get; set; }
}
