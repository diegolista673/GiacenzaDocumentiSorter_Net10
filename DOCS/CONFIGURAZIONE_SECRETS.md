# Configurazione Secrets - GiacenzaSorterRm

## ?? Overview Ambienti

Il progetto supporta **3 ambienti** con configurazioni separate:

| Ambiente | Scopo | LDAP/AD | Database | Configuration |
|----------|-------|---------|----------|---------------|
| **LocalDev** | Sviluppo locale senza AD | ? No | DB Locale (SQL Express) | User Secrets + appsettings.LocalDev.json |
| **TestDev** | Sviluppo con test AD | ? Si | DB Test Remoto | User Secrets + appsettings.TestDev.json |
| **Production** | Produzione | ? Si | DB Produzione | Environment Variables + appsettings.json |

---

## ?? Quick Start

### Setup Rapido LocalDev (Senza LDAP)
```bash
# 1. Setup automatico
setup-localdev.bat

# 2. Avvia applicazione
run-localdev.bat
```

### Setup Rapido TestDev (Con LDAP)
```bash
# 1. Setup automatico
setup-testdev.bat

# 2. Avvia applicazione
run-testdev.bat
```

---

## ?? Setup Ambiente 1: LocalDev (Senza LDAP)

### Caratteristiche
- ? Solo utenti ESTERNI (password hash nel DB)
- ? Nessuna connessione Active Directory
- ?? Database locale su SQL Express
- ?? Lockout più permissivo (10 tentativi, 5 minuti)

### Setup Automatico (CONSIGLIATO)

```bash
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm
setup-localdev.bat
```

Lo script ti guiderà nella configurazione interattiva.

### Setup Manuale

```bash
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm

# Inizializza User Secrets
dotnet user-secrets init

# Configura Connection String per DB Locale
# Opzione 1: SQL Express con Windows Authentication
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_LOCAL;Integrated Security=True;TrustServerCertificate=True;"

# Opzione 2: SQL Server con credenziali
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GIACENZA_SORTER_RM_LOCAL;User Id=dev_user;Password=dev_password;TrustServerCertificate=True;"

# Verifica
dotnet user-secrets list
```

### Avvio Applicazione

**Metodo 1 - Script (CONSIGLIATO):**
```bash
run-localdev.bat
```

**Metodo 2 - Command Line:**
```bash
# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="LocalDev"
dotnet run

# Windows (CMD)
set ASPNETCORE_ENVIRONMENT=LocalDev
dotnet run
```

**Metodo 3 - Visual Studio:**
1. Apri `Properties\launchSettings.json`
2. Seleziona profilo "LocalDev" dal menu dropdown
3. Premi F5

### Test Login LocalDev
- ? **Utenti ESTERNI**: Funzionano (password hash nel DB)
- ? **Utenti AD**: Falliscono gracefully (AD disabilitato)

---

## ?? Setup Ambiente 2: TestDev (Con LDAP)

### Caratteristiche
- ? Utenti ESTERNI (password hash)
- ? Utenti ACTIVE DIRECTORY (LDAP test)
- ?? Database test remoto
- ?? Lockout standard (5 tentativi, 10 minuti)

### Setup Automatico (CONSIGLIATO)

```bash
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm
setup-testdev.bat
```

### Setup Manuale

```bash
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm

# Inizializza User Secrets
dotnet user-secrets init

# Connection String per DB Test
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=TEST-SQL-SERVER;Database=GIACENZA_SORTER_RM_TEST;User Id=test_user;Password=test_password;TrustServerCertificate=True;"

# Active Directory Service Account
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Username" "POSTEL\\svc_giacenza_test"
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "YOUR_TEST_AD_PASSWORD"

# Opzionale: Override LDAP Path
dotnet user-secrets set "ActiveDirectory:LdapPath" "LDAP://custom-test-dc.postel.it"

# Verifica
dotnet user-secrets list
```

### Avvio Applicazione

**Metodo 1 - Script (CONSIGLIATO):**
```bash
run-testdev.bat
```

**Metodo 2 - Command Line:**
```bash
set ASPNETCORE_ENVIRONMENT=TestDev
dotnet run
```

### Test Login TestDev
- ? **Utenti ESTERNI**: Funzionano
- ? **Utenti AD**: Autenticazione tramite LDAP test (test-dc.postel.it)

---

## ?? Switching Tra Ambienti

### Metodo 1: Usa Script Dedicati
```bash
# LocalDev
run-localdev.bat

# TestDev
run-testdev.bat
```

### Metodo 2: Cambia Environment Variable
```bash
# Switch a LocalDev
set ASPNETCORE_ENVIRONMENT=LocalDev
dotnet run

# Switch a TestDev
set ASPNETCORE_ENVIRONMENT=TestDev
dotnet run
```

### Metodo 3: Visual Studio Launch Profiles
Modifica `Properties\launchSettings.json`:
```json
{
  "profiles": {
    "GiacenzaSorterRm (LocalDev)": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "LocalDev"
      }
    },
    "GiacenzaSorterRm (TestDev)": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "TestDev"
      }
    }
  }
}
```

---

## ?? Gestione User Secrets Multi-Ambiente

### Limitazione User Secrets

**IMPORTANTE**: User Secrets è **unico per progetto** (stesso file `secrets.json` per tutti gli ambienti).

### Soluzione: Switching Connection String

Quando switchi tra ambienti, devi cambiare anche la connection string:

```bash
# Switch a LocalDev
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_LOCAL;Integrated Security=True;TrustServerCertificate=True;"
set ASPNETCORE_ENVIRONMENT=LocalDev

# Switch a TestDev
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=TEST-SQL;Database=GIACENZA_SORTER_RM_TEST;User Id=test;Password=pwd;TrustServerCertificate=True;"
set ASPNETCORE_ENVIRONMENT=TestDev
```

**?? Suggerimento**: Usa gli script `setup-localdev.bat` e `setup-testdev.bat` per gestire automaticamente il cambio.

---

## ?? Struttura Files di Configurazione

```
GiacenzaSorterRm/
??? appsettings.json                 # Base comune
??? appsettings.LocalDev.json        # ? Override LocalDev (NUOVO)
??? appsettings.TestDev.json         # ? Override TestDev (NUOVO)
??? appsettings.Production.json      # Override Production
??? Properties/
?   ??? launchSettings.json          # Profili Visual Studio
??? run-localdev.bat                 # ? Script avvio LocalDev (NUOVO)
??? run-testdev.bat                  # ? Script avvio TestDev (NUOVO)
??? setup-localdev.bat               # ? Setup LocalDev (NUOVO)
??? setup-testdev.bat                # ? Setup TestDev (NUOVO)
??? %APPDATA%\Microsoft\UserSecrets\<id>\secrets.json  # User Secrets
```

### Priorità Caricamento Configurazione

.NET carica le configurazioni in questo ordine (l'ultimo fa override):

1. `appsettings.json`
2. `appsettings.{Environment}.json` (es: `appsettings.LocalDev.json`)
3. **User Secrets** (solo in Development/LocalDev/TestDev)
4. Environment Variables
5. Command-line arguments

---

## ?? Testing Configurazione

### Verifica Ambiente Corrente
```bash
# Mostra ambiente attivo
echo %ASPNETCORE_ENVIRONMENT%

# Mostra User Secrets
dotnet user-secrets list
```

### Test LocalDev
```bash
# 1. Avvia
run-localdev.bat

# 2. Verifica log startup
# Dovrebbe mostrare:
# - Hosting environment: LocalDev
# - Active Directory: DISABILITATO

# 3. Test login
# - Utente ESTERNO: ? Funziona
# - Utente AD: ? Fallisce (atteso)
```

### Test TestDev
```bash
# 1. Avvia
run-testdev.bat

# 2. Verifica log startup
# Dovrebbe mostrare:
# - Hosting environment: TestDev
# - Active Directory: ABILITATO
# - LDAP Path: LDAP://test-dc.postel.it

# 3. Test login
# - Utente ESTERNO: ? Funziona
# - Utente AD: ? Funziona (LDAP)
```

---

## ?? Troubleshooting

### Errore: "Active Directory LDAP path not configured"

**In LocalDev**: ? Normale - AD è disabilitato intenzionalmente

**In TestDev**: ? Problema di configurazione
```bash
# Verifica appsettings.TestDev.json
type appsettings.TestDev.json

# Verifica User Secrets
dotnet user-secrets list

# Verifica environment
echo %ASPNETCORE_ENVIRONMENT%
```

### Errore: "Connection string not found"
```bash
# Verifica User Secrets
dotnet user-secrets list

# Se vuoto, ri-esegui setup
setup-localdev.bat   # oppure
setup-testdev.bat
```

### Quale ambiente sto usando?

Aggiungi log temporaneo in `Startup.cs`:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    Console.WriteLine("========================================");
    Console.WriteLine($"ENVIRONMENT: {env.EnvironmentName}");
    Console.WriteLine("========================================");
    
    // ... resto codice
}
```

### Database non raggiungibile

**LocalDev:**
```bash
# Verifica SQL Express attivo
sc query MSSQL$SQLEXPRESS

# Testa connessione
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@VERSION"
```

**TestDev:**
```bash
# Verifica connettività server test
ping TEST-SQL-SERVER

# Testa connessione
sqlcmd -S TEST-SQL-SERVER -U test_user -P test_password -Q "SELECT @@VERSION"
```

---

## ?? Configurazione Produzione (IIS)

Per la produzione, configurare le variabili d'ambiente in IIS Manager:

1. Apri IIS Manager
2. Application Pools ? Seleziona pool ? Advanced Settings ? Configuration Editor
3. Section: system.applicationHost/applicationPools
4. Aggiungi le seguenti variabili:

```
ConnectionStrings__DefaultConnection = Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=PASSWORD_PROD;TrustServerCertificate=True;
ActiveDirectory__ServiceAccount__Username = POSTEL\svc_giacenza
ActiveDirectory__ServiceAccount__Password = PASSWORD_PROD
ASPNETCORE_ENVIRONMENT = Production
```

**Nota**: Usa `__` (doppio underscore) per separare i livelli gerarchici.

---

## ?? Checklist Setup

### LocalDev
- [ ] File `appsettings.LocalDev.json` presente
- [ ] Script `setup-localdev.bat` eseguito
- [ ] User Secrets configurati con DB locale
- [ ] `run-localdev.bat` funziona
- [ ] Testato login utenti ESTERNI
- [ ] Verificato che utenti AD falliscono gracefully

### TestDev
- [ ] File `appsettings.TestDev.json` presente
- [ ] Script `setup-testdev.bat` eseguito
- [ ] User Secrets configurati con DB test e AD
- [ ] `run-testdev.bat` funziona
- [ ] Testato login utenti ESTERNI
- [ ] Testato login utenti AD
- [ ] LDAP test raggiungibile

---

## ?? Best Practices

### DO ?
- ? Usa script dedicati per ogni ambiente
- ? Cambia User Secrets quando switchi ambiente
- ? Documenta quale DB usa ogni ambiente
- ? Testa sempre dopo cambio ambiente
- ? Usa nomi database diversi (LOCAL vs TEST)

### DON'T ?
- ? Non usare stesso DB per LocalDev e TestDev
- ? Non committare `secrets.*.json` files
- ? Non mettere password in `appsettings.*.json`
- ? Non dimenticare di cambiare connection string quando switchi

---

## ?? Quick Reference

| Azione | Comando |
|--------|---------|
| Setup LocalDev | `setup-localdev.bat` |
| Setup TestDev | `setup-testdev.bat` |
| Avvia LocalDev | `run-localdev.bat` |
| Avvia TestDev | `run-testdev.bat` |
| Verifica ambiente | `echo %ASPNETCORE_ENVIRONMENT%` |
| Lista User Secrets | `dotnet user-secrets list` |
| Cambia connection string | `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."` |

---

## ?? Documentazione Aggiuntiva

- [ASP.NET Core Environments](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Safe Storage of App Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

---

## Security Checklist

- [ ] User Secrets configurati per LocalDev
- [ ] User Secrets configurati per TestDev
- [ ] Variabili d'ambiente configurate in IIS per Production
- [ ] File `MyConnections.cs` rimosso dal progetto
- [ ] `web.config` con secrets aggiunto a `.gitignore`
- [ ] Password cambiate se erano committate in Git
- [ ] Script `.bat` funzionanti per entrambi gli ambienti
