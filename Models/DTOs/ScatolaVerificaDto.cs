using System;

namespace GiacenzaSorterRm.Models.DTOs
{
// Crea un DTO semplice per la verifica della scatola
public class ScatolaVerificaDto
{
    public int IdScatola { get; set; }
    public string? Scatola { get; set; }                    
    public DateTime? DataSorter { get; set; }               
    public DateTime? DataNormalizzazione { get; set; }      // ✅ NULLABLE
    public int? IdStato { get; set; }                       // ✅ NULLABLE per sicurezza
    public string? OperatoreSorter { get; set; }            
    public string? Note { get; set; }                       
    public int? IdCentroSorterizzazione { get; set; }   
}
}
