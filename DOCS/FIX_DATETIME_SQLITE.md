# ?? Fix DateTime Format Exception in SQLite

## ?? Problema

Quando si utilizza SQLite in ambiente LocalDev, si verifica un'eccezione `System.FormatException` durante il caricamento dei dati dalla tabella `Commesses`:

```
Exception Message: String '17/08/2020 00:00:00' was not recognized as a valid DateTime.
```

### Causa Root

SQLite memorizza i valori `DateTime` come stringhe `TEXT`. Il database conteneva date in formato italiano (`dd/MM/yyyy HH:mm:ss`), ma Entity Framework SQLite Provider non riusciva a deserializzarle correttamente perché si aspettava il formato ISO 8601 predefinito (`yyyy-MM-dd HH:mm:ss`).

L'applicazione è configurata con cultura italiana (`it-IT` in `Startup.cs`), ma il DbContext SQLite necessitava di un converter personalizzato per gestire entrambi i formati.

## ? Soluzione Applicata

### 1. Value Converter Personalizzato

Creati due converter in `Data/GiacenzaSorterSqliteContext.cs`:

#### `DateTimeToStringConverter`
Gestisce la conversione bidirezionale per `DateTime`:
```csharp
public class DateTimeToStringConverter : ValueConverter<DateTime, string>
{
    // Scrive: converte DateTime in formato ISO 8601
    v => v.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
    
    // Legge: supporta formati multipli:
    // - yyyy-MM-dd HH:mm:ss (ISO 8601)
    // - dd/MM/yyyy HH:mm:ss (Italiano)
    // - dd/MM/yyyy (Italiano corto)
    // - yyyy-MM-dd (ISO corto)
}
```

#### `NullableDateTimeToStringConverter`
Stessa logica per `DateTime?` nullable.

### 2. Configurazione Globale in DbContext

Aggiunto metodo `ConfigureConventions` che applica automaticamente i converter a tutte le proprietà DateTime:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .Properties<DateTime>()
        .HaveConversion<DateTimeToStringConverter>();

    configurationBuilder
        .Properties<DateTime?>()
        .HaveConversion<NullableDateTimeToStringConverter>();
}
```

### 3. Logica di Parsing Multi-Formato

Il converter prova il parsing in questo ordine:
1. **Cultura italiana** (`it-IT`) - per gestire `dd/MM/yyyy HH:mm:ss`
2. **Cultura invariante** - per gestire `yyyy-MM-dd HH:mm:ss`
3. **Formati espliciti** - fallback con array di formati predefiniti

Questo garantisce la **compatibilità bidirezionale**:
- ? Legge date esistenti in formato italiano dal database
- ? Scrive nuove date in formato ISO 8601 standard
- ? Supporta entrambi i formati in query e operazioni CRUD

## ?? Risultato

? Entity Framework deserializza correttamente le date italiane esistenti in SQLite  
? Nuove date vengono salvate in formato ISO 8601 per compatibilità futura  
? Tutte le query su tabelle con colonne DateTime funzionano correttamente  
? Non sono necessarie modifiche al codice esistente o alle entità  
? Soluzione robusta che gestisce formati misti nel database

## ?? Tabelle Interessate

Le seguenti tabelle beneficiano automaticamente di questa correzione:

- `Commesse` - `DataCreazione`
- `Bancali` - `DataAccettazioneBancale`, `DataInvioAltroCentro`, `DataSorter`
- `BancaliDispacci` - `DataAssociazione`
- `Contenitori` - `DataCreazione`
- `Piattaforme` - `DataCreazione`
- `Scatole` - `DataNormalizzazione`, `DataSorter`, `DataCambioGiacenza`, `DataMacero`
- `Tipologie` - `DataCreazione`

## ?? Verifica

Per verificare che il fix funzioni correttamente:

1. Avvia l'applicazione in ambiente LocalDev:
   ```powershell
   dotnet run --environment LocalDev
   ```

2. Naviga alla pagina Accettazione (`/PagesAccettazione/Create`)

3. Verifica che il caricamento della ComboBox delle Commesse avvenga senza errori

4. Controlla i log per confermare l'assenza di eccezioni di formato DateTime

## ??? Dettagli Tecnici

### Perché questa soluzione?

La prima implementazione tentava di usare `DateTimeKind` e `DateTimeFormat` nella connection string, ma questi parametri:
- ? Non sono supportati nelle versioni recenti di `Microsoft.Data.Sqlite`
- ? Causavano `ArgumentException: Connection string keyword 'datetimekind' is not supported`

La soluzione finale con **Value Converters**:
- ? È il metodo ufficiale raccomandato da EF Core per custom type conversions
- ? Funziona con tutte le versioni di EF Core e SQLite
- ? Permette controllo granulare su lettura e scrittura
- ? Si applica automaticamente a tutte le proprietà DateTime tramite conventions

### Esempio di Utilizzo

```csharp
// Lettura - supporta entrambi i formati:
var commesse = await _context.Commesses.ToListAsync();
// "17/08/2020 00:00:00" (italiano) ? DateTime(2020, 8, 17)
// "2020-08-17 00:00:00" (ISO) ? DateTime(2020, 8, 17)

// Scrittura - salva sempre in ISO 8601:
var nuovaCommessa = new Commesse 
{ 
    Commessa = "Test",
    DataCreazione = DateTime.Now 
};
await _context.Commesses.AddAsync(nuovaCommessa);
await _context.SaveChangesAsync();
// Database: "2025-01-24 10:30:00"
```

## ?? Riferimenti

- [EF Core Value Converters](https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions)
- [SQLite DateTime Formats](https://www.sqlite.org/lang_datefunc.html)
- [Entity Framework Core SQLite Provider](https://docs.microsoft.com/en-us/ef/core/providers/sqlite/)
- [Model Configuration in EF Core](https://docs.microsoft.com/en-us/ef/core/modeling/model-configuration)

## ?? Note

- Questa configurazione è specifica per l'ambiente LocalDev con SQLite
- L'ambiente di produzione continua a utilizzare SQL Server che gestisce nativamente i tipi DateTime
- Le nuove date vengono sempre salvate in formato ISO 8601 per garantire compatibilità
- Il converter gestisce automaticamente anche formati con millisecondi (`yyyy-MM-dd HH:mm:ss.fff`)

## ?? Attenzione

Se si ricrea il database SQLite da zero:
- È raccomandato esportare le date da SQL Server in formato ISO 8601
- In alternativa, il converter continuerà a gestire correttamente anche formati misti
- Per miglior performance, standardizzare su un unico formato (ISO 8601)

---

**Data Fix**: 2025-01-24  
**Versione**: 2.0 (Updated - Value Converters)  
**Severity**: High (blocca il funzionamento dell'applicazione in LocalDev)  
**Status**: ? Risolto
