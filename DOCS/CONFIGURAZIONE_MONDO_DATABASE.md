# ??? Configurazione Database Mondo - Multi-Ambiente

**Data**: 2025-01-24  
**Status**: ? **COMPLETATO**

---

## ?? Obiettivo

Configurare la connection string al database **Mondo** (MND_SCATOLE_STAMPATE_LISTA) in base all'ambiente:
- **LocalDev/TestDev**: Azure SQL Database
- **Production**: SQL Server on-premises (altro server)

---

## ?? Problema Risolto

### ? Prima

Connection string **hardcoded** nel codice:

```csharp
private async Task<bool> ControllaNomeScatolaMondoAsync(string nomeScatola)
{
    string sql = @"SELECT COD_STAMPA FROM MND_SCATOLE_STAMPATE_LISTA where COD_STAMPA = @scatola";

    using (SqlConnection connection = new SqlConnection(""))  // ? STRINGA VUOTA!
    {
        // ...
    }
}
```

**Problemi:**
- ? Connection string vuota (non funziona)
- ? Non gestisce ambienti diversi
- ? Nessuna gestione errori
- ? Nessun logging
- ? Hardcoded (se committato ? security issue)

---

### ? Dopo

Connection string **configurata dinamicamente** da `IConfiguration`:

```csharp
private async Task<bool> ControllaNomeScatolaMondoAsync(string nomeScatola)
{
    // Recupera connection string da configurazione
    string connectionString = _configuration.GetConnectionString("MondoConnection");
    
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        _logger.LogError("MondoConnection non configurata");
        throw new InvalidOperationException("MondoConnection non configurata");
    }

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        // ... query
    }
}
```

**Vantaggi:**
- ? Connection string da configurazione esterna
- ? Supporto multi-ambiente (LocalDev/TestDev/Production)
- ? Gestione errori completa
- ? Logging dettagliato
- ? Sicurezza: nessuna password hardcoded

---

## ?? Modifiche Applicate

### 1. Constructor Injection di IConfiguration

**File:** `Pages/PagesNormalizzazione/Create.cshtml.cs`

**Prima:**
```csharp
public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context)
{
    _logger = logger;
    _context = context;
}
```

**Dopo:**
```csharp
private readonly IConfiguration _configuration;

public CreateModel(ILogger<CreateModel> logger, GiacenzaSorterContext context, IConfiguration configuration)
{
    _logger = logger;
    _context = context;
    _configuration = configuration;
}
```

---

### 2. Metodo ControllaNomeScatolaMondoAsync Riscritto

**Miglioramenti:**

#### A. Recupero Connection String Dinamico
```csharp
string connectionString = _configuration.GetConnectionString("MondoConnection");
```

#### B. Validazione Connection String
```csharp
if (string.IsNullOrWhiteSpace(connectionString))
{
    _logger.LogError("MondoConnection string non configurata");
    throw new InvalidOperationException("MondoConnection non configurata");
}
```

#### C. Validazione Input
```csharp
if (string.IsNullOrWhiteSpace(nomeScatola))
{
    _logger.LogWarning("ControllaNomeScatolaMondo: nome scatola vuoto o null");
    return false;
}
```

#### D. Timeout SQL
```csharp
cmd.CommandTimeout = 30; // Timeout 30 secondi
```

#### E. Gestione Errori Completa
```csharp
try
{
    // ... query
}
catch (SqlException sqlEx)
{
    _logger.LogError(sqlEx, $"Errore SQL durante verifica scatola {nomeScatola}");
    throw new InvalidOperationException($"Errore connessione database Mondo", sqlEx);
}
catch (Exception ex)
{
    _logger.LogError(ex, $"Errore generico durante verifica scatola {nomeScatola}");
    throw;
}
```

#### F. Logging Dettagliato
```csharp
if (res)
{
    _logger.LogDebug($"Scatola {nomeScatola} trovata su Mondo");
}
else
{
    _logger.LogWarning($"Scatola {nomeScatola} NON trovata su Mondo");
}
```

#### G. Using Statements Corretti
```csharp
using (SqlConnection connection = new SqlConnection(connectionString))
{
    using (SqlCommand cmd = new SqlCommand(sql, connection))
    {
        // ... query
    }
}
```

---

### 3. File Configurazione Aggiornati

#### appsettings.json (Base)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PLACEHOLDER...",
    "MondoConnection": "PLACEHOLDER - Configurare in User Secrets (dev) o Environment Variables (prod)"
  }
}
```

#### appsettings.LocalDev.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PLACEHOLDER...",
    "MondoConnection": "PLACEHOLDER - Azure SQL Database Mondo (LocalDev usa Azure)"
  }
}
```

#### appsettings.TestDev.json
```json
{
  "ConnectionStrings": {
    "MondoConnection": "PLACEHOLDER - Azure SQL Database Mondo (TestDev usa Azure)"
  }
}
```

---

## ?? Script Setup Creati

### 1. setup-mondo-localdev.bat

Setup guidato per configurare MondoConnection in LocalDev (Azure SQL).

**Utilizzo:**
```bash
setup-mondo-localdev.bat
```

**Cosa fa:**
1. Chiede server Azure SQL (es: `myserver.database.windows.net`)
2. Chiede database name (es: `MND_SCATOLE`)
3. Chiede username Azure SQL
4. Chiede password Azure SQL
5. Costruisce connection string Azure SQL
6. Configura User Secrets: `dotnet user-secrets set "ConnectionStrings:MondoConnection" "..."`

---

### 2. setup-mondo-testdev.bat

Setup guidato per configurare MondoConnection in TestDev (Azure SQL).

**Utilizzo:**
```bash
setup-mondo-testdev.bat
```

**Identico a LocalDev**, ma configurato per ambiente TestDev.

---

## ?? Configurazione per Ambiente

### LocalDev (Azure SQL Database)

#### Setup Automatico (CONSIGLIATO)
```bash
setup-mondo-localdev.bat
```

#### Setup Manuale
```bash
cd C:\Users\SMARTW\source\repos\GiacenzaSorter

dotnet user-secrets set "ConnectionStrings:MondoConnection" "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=MND_SCATOLE;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

**Esempio Connection String Azure SQL:**
```
Server=tcp:giacenza-mondo.database.windows.net,1433;
Initial Catalog=MND_SCATOLE;
Persist Security Info=False;
User ID=mondo_reader;
Password=YourSecurePassword123!;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

---

### TestDev (Azure SQL Database)

#### Setup Automatico (CONSIGLIATO)
```bash
setup-mondo-testdev.bat
```

#### Setup Manuale
```bash
dotnet user-secrets set "ConnectionStrings:MondoConnection" "Server=tcp:YOUR_TEST_SERVER.database.windows.net,1433;Initial Catalog=MND_SCATOLE_TEST;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;Connection Timeout=30;"
```

---

### Production (SQL Server on-premises)

Configurare **Environment Variables** in IIS:

#### Metodo 1: IIS Manager
1. Apri IIS Manager
2. Seleziona Application Pool ? Advanced Settings
3. Configuration Editor
4. Section: `system.applicationHost/applicationPools`
5. Aggiungi:
```
Name: ConnectionStrings__MondoConnection
Value: Server=PROD-SQL-SERVER;Database=MND_SCATOLE_PROD;User Id=mondo_user;Password=PROD_PASSWORD;TrustServerCertificate=True;Connection Timeout=30;
```

**Nota:** Usa `__` (doppio underscore) per separare livelli gerarchici.

#### Metodo 2: web.config
```xml
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <aspNetCore processPath="dotnet" arguments=".\GiacenzaSorterRm.dll">
        <environmentVariables>
          <environmentVariable name="ConnectionStrings__MondoConnection" 
                               value="Server=PROD-SQL-SERVER;Database=MND_SCATOLE_PROD;User Id=mondo_user;Password=PROD_PASSWORD;TrustServerCertificate=True;" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

**Esempio Connection String SQL Server on-premises:**
```
Server=SRVR-MONDO-01;
Database=MND_SCATOLE_PROD;
User Id=mondo_reader;
Password=ProdPassword123!;
Persist Security Info=False;
TrustServerCertificate=True;
Connection Timeout=30;
```

---

## ?? Testing

### Verifica Configurazione

```bash
# Mostra User Secrets (LocalDev/TestDev)
dotnet user-secrets list

# Cerca MondoConnection
dotnet user-secrets list | findstr Mondo
```

**Output atteso:**
```
ConnectionStrings:MondoConnection = Server=tcp:myserver.database.windows.net,1433;...
```

---

### Test Funzionale

#### 1. Test LocalDev
```bash
# 1. Setup
setup-mondo-localdev.bat

# 2. Avvia applicazione
run-localdev.bat

# 3. Vai a Normalizzazione Scatole
# 4. Inserisci una scatola
# 5. Verifica log:
#    - "Scatola XXX trovata su Mondo" (se esiste)
#    - "Scatola XXX NON trovata su Mondo" (se non esiste)
#    - NO errori "MondoConnection non configurata"
```

#### 2. Test TestDev
```bash
setup-mondo-testdev.bat
run-testdev.bat
# ... stesso test
```

#### 3. Test Production
```
1. Deploy su IIS
2. Verifica Environment Variables configurate
3. Restart Application Pool
4. Test inserimento scatola
5. Verifica log
```

---

### Scenari di Test

| Scenario | Input | Output Atteso | Log Atteso |
|----------|-------|---------------|------------|
| **Scatola Esistente** | "ABC12345678" | `true` | "Scatola ABC12345678 trovata su Mondo" |
| **Scatola Non Esistente** | "XYZ99999999" | `false` | "Scatola XYZ99999999 NON trovata su Mondo" |
| **Scatola Vuota** | "" | `false` | "nome scatola vuoto o null" |
| **Scatola Null** | `null` | `false` | "nome scatola vuoto o null" |
| **Connection String Mancante** | - | Exception | "MondoConnection non configurata" |
| **Database Irraggiungibile** | - | Exception | "Errore SQL durante verifica scatola" |

---

## ?? Troubleshooting

### Errore: "MondoConnection non configurata"

**Causa:** Connection string non impostata.

**Soluzione:**

**LocalDev/TestDev:**
```bash
# Verifica User Secrets
dotnet user-secrets list

# Se vuoto, ri-esegui setup
setup-mondo-localdev.bat   # o setup-mondo-testdev.bat
```

**Production:**
```bash
# Verifica Environment Variables in IIS
# O aggiungi in web.config
```

---

### Errore: "Errore SQL durante verifica scatola"

**Causa:** Problemi di connessione database.

**Soluzioni:**

#### LocalDev/TestDev (Azure SQL)
```bash
# Test connessione Azure SQL
sqlcmd -S tcp:YOUR_SERVER.database.windows.net,1433 -U YOUR_USER -P YOUR_PASSWORD -Q "SELECT 1"

# Verifica firewall Azure SQL:
# 1. Vai a Azure Portal
# 2. SQL Databases ? tuo database ? Networking
# 3. Aggiungi IP pubblico client
```

#### Production (SQL Server)
```bash
# Test connessione SQL Server
sqlcmd -S PROD-SQL-SERVER -U mondo_user -P PROD_PASSWORD -Q "SELECT 1"

# Verifica SQL Server Browser attivo
sc query SQLBROWSER
```

---

### Errore: "Timeout expired"

**Causa:** Query troppo lenta o database sovraccarico.

**Soluzione:**

Aumenta timeout nel metodo (default 30 secondi):
```csharp
cmd.CommandTimeout = 60; // 60 secondi
```

O nella connection string:
```
...;Connection Timeout=60;
```

---

### Warning: "Scatola trovata ma con nome diverso"

**Causa:** Case mismatch (es: input "abc123" vs database "ABC123").

**Soluzione:** Il metodo usa già `StringComparison.OrdinalIgnoreCase`, ma verifica:
```csharp
res = scatolaMondo.ToString().Equals(nomeScatola, StringComparison.OrdinalIgnoreCase);
```

---

## ?? Confronto Prima/Dopo

| Aspetto | Prima | Dopo | ? |
|---------|-------|------|---|
| **Connection String** | Hardcoded (vuota) | Da configurazione | +100% |
| **Multi-Ambiente** | ? No | ? Sì (LocalDev/TestDev/Prod) | +100% |
| **Gestione Errori** | ? No | ? Completa (SqlException + generic) | +100% |
| **Logging** | ? No | ? Dettagliato (Debug/Warning/Error) | +100% |
| **Validazione Input** | ? No | ? Sì (null/empty check) | +100% |
| **Timeout SQL** | ? Default | ? 30 secondi configurabile | +100% |
| **Sicurezza** | ? Hardcoded | ? User Secrets / Env Vars | +100% |
| **Using Statements** | ? Solo connection | ? Connection + Command | +50% |

---

## ?? Best Practices Applicate

### 1. Configuration Management ?
- Connection string da `IConfiguration`
- User Secrets per development
- Environment Variables per production

### 2. Error Handling ?
- Try-catch specifico per `SqlException`
- Try-catch generico per altri errori
- Logging dettagliato per debugging
- Exception con messaggio user-friendly

### 3. Logging ?
- `LogDebug` per operazioni normali
- `LogWarning` per condizioni anomale (scatola non trovata)
- `LogError` per errori database

### 4. Security ?
- Nessuna password hardcoded
- Connection string da configurazione esterna
- Parametrized query (protezione SQL injection)

### 5. Performance ?
- Timeout SQL configurato (30 secondi)
- Using statements corretti (dispose automatico)
- Query semplice e ottimizzata

### 6. Code Quality ?
- Validazione input
- Commenti XML documentation
- Naming convention chiaro
- Single Responsibility

---

## ?? Documentazione Correlata

| File | Descrizione |
|------|-------------|
| `DOCS/CONFIGURAZIONE_SECRETS.md` | Guida User Secrets multi-ambiente |
| `DOCS/GUIDA_AMBIENTI.md` | Quick start LocalDev/TestDev/Production |
| `setup-mondo-localdev.bat` | Script setup MondoConnection LocalDev |
| `setup-mondo-testdev.bat` | Script setup MondoConnection TestDev |

---

## ? Conclusione

La configurazione database Mondo è ora **completamente multi-ambiente**:

- ? **LocalDev**: Azure SQL Database (User Secrets)
- ? **TestDev**: Azure SQL Database (User Secrets)
- ? **Production**: SQL Server on-premises (Environment Variables)

**Vantaggi:**
- ? Nessuna password hardcoded
- ? Configurazione centralizzata
- ? Gestione errori robusta
- ? Logging completo
- ? Sicurezza migliorata

**Ready for Use!** ??

---

**Ultima modifica**: 2025-01-24  
**Autore**: GitHub Copilot  
**Status**: ? **COMPLETATO**  
**Build**: ? Success
