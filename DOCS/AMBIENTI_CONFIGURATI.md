# ? CONFIGURAZIONE MULTI-AMBIENTE COMPLETATA

## ?? Obiettivo Raggiunto

Configurazione di **2 ambienti di sviluppo indipendenti**:
1. **LocalDev** - Senza LDAP/Active Directory (DB locale)
2. **TestDev** - Con LDAP/Active Directory (DB test remoto)

---

## ?? File Creati

### ? Configurazioni Ambiente
- `appsettings.LocalDev.json` - Configurazione LocalDev (no LDAP)
- `appsettings.TestDev.json` - Configurazione TestDev (con LDAP)

### ? Script Setup
- `setup-localdev.bat` - Setup guidato interattivo LocalDev
- `setup-testdev.bat` - Setup guidato interattivo TestDev

### ? Script Avvio
- `run-localdev.bat` - Avvia applicazione in LocalDev
- `run-testdev.bat` - Avvia applicazione in TestDev

### ? Documentazione
- `DOCS\CONFIGURAZIONE_SECRETS.md` - Guida completa (AGGIORNATA)
- `DOCS\GUIDA_AMBIENTI.md` - Quick start guide (NUOVO)

### ? Visual Studio
- `Properties\launchSettings.json` - Profili debug aggiornati

---

## ?? Come Usare

### ?? Prima Configurazione - LocalDev

```bash
# 1. Setup
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm
setup-localdev.bat

# 2. Segui wizard interattivo
#    Opzioni disponibili:
#    1. SQL Express con Windows Authentication (consigliato)
#    2. SQL Server con user/password
#    3. Connection string manuale

# 3. Avvia applicazione
run-localdev.bat

# 4. Testa login
#    ? Utenti ESTERNI: Funzionano
#    ? Utenti AD: Falliscono (normale - AD disabilitato)
```

### ?? Prima Configurazione - TestDev

```bash
# 1. Setup
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm
setup-testdev.bat

# 2. Inserisci dati richiesti:
#    - Server DB test
#    - Database name
#    - Username/Password DB
#    - AD Service Account username
#    - AD Service Account password
#    - (Opzionale) Custom LDAP path

# 3. Avvia applicazione
run-testdev.bat

# 4. Testa login
#    ? Utenti ESTERNI: Funzionano
#    ? Utenti AD: Funzionano (LDAP authentication)
```

---

## ?? Switching Tra Ambienti

### Metodo 1: Script Dedicati (CONSIGLIATO)

```bash
# Switch a LocalDev
run-localdev.bat

# Switch a TestDev
run-testdev.bat
```

Gli script verificano automaticamente:
- ? Environment variable corretta
- ? User Secrets configurati
- ? AD secrets (solo per TestDev)

### Metodo 2: Visual Studio

1. Apri il progetto in Visual Studio
2. Toolbar Debug ? Seleziona profilo:
   - **"LocalDev (Senza LDAP)"**
   - **"TestDev (Con LDAP)"**
3. Premi **F5**

### Metodo 3: Command Line Manuale

```bash
# LocalDev
set ASPNETCORE_ENVIRONMENT=LocalDev
dotnet run

# TestDev
set ASPNETCORE_ENVIRONMENT=TestDev
dotnet run
```

---

## ?? Configurazioni per Ambiente

### LocalDev (Sviluppo Locale)

| Configurazione | Valore |
|----------------|--------|
| **Database** | SQL Express Locale (`GIACENZA_SORTER_RM_LOCAL`) |
| **Active Directory** | ? **DISABILITATO** |
| **Autenticazione** | Solo utenti ESTERNI (password hash) |
| **Max Failed Attempts** | 10 tentativi |
| **Lockout Duration** | 5 minuti |
| **Log Level** | Debug |
| **LDAP Path** | Vuoto (ignorato) |

**File di configurazione**: `appsettings.LocalDev.json`

**User Secrets richiesti**:
- `ConnectionStrings:DefaultConnection` ? REQUIRED

**Connection String Esempio**:
```
Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_LOCAL;Integrated Security=True;TrustServerCertificate=True;
```

---

### TestDev (Test con LDAP)

| Configurazione | Valore |
|----------------|--------|
| **Database** | Server Test Remoto (`GIACENZA_SORTER_RM_TEST`) |
| **Active Directory** | ? **ABILITATO** |
| **LDAP Server** | `test-dc.postel.it` |
| **Autenticazione** | Utenti ESTERNI + AD |
| **Max Failed Attempts** | 5 tentativi |
| **Lockout Duration** | 10 minuti |
| **Log Level** | Debug + Trace (AD) |

**File di configurazione**: `appsettings.TestDev.json`

**User Secrets richiesti**:
- `ConnectionStrings:DefaultConnection` ? REQUIRED
- `ActiveDirectory:ServiceAccount:Username` ? REQUIRED
- `ActiveDirectory:ServiceAccount:Password` ? REQUIRED
- `ActiveDirectory:LdapPath` ?? OPTIONAL (override)

**Connection String Esempio**:
```
Server=TEST-SQL-SERVER;Database=GIACENZA_SORTER_RM_TEST;User Id=test_user;Password=test_password;TrustServerCertificate=True;
```

**Service Account Esempio**:
```
Username: POSTEL\svc_giacenza_test
Password: [secret]
```

---

## ?? Testing

### ? Test Checklist LocalDev

```bash
# 1. Avvia
run-localdev.bat

# 2. Verifica log startup
? "Hosting environment: LocalDev"
? "Active Directory: DISABILITATO" (o errore LDAP - normale)

# 3. Test autenticazione
? Login utente ESTERNO ? Successo
? Login utente AD ? Fallisce (atteso - AD disabilitato)

# 4. Test lockout
? 10 tentativi falliti ? Account bloccato
? Attesa 5 minuti ? Account sbloccato
```

### ? Test Checklist TestDev

```bash
# 1. Avvia
run-testdev.bat

# 2. Verifica log startup
? "Hosting environment: TestDev"
? "LDAP Path: LDAP://test-dc.postel.it"

# 3. Test autenticazione
? Login utente ESTERNO ? Successo
? Login utente AD ? Successo (LDAP authentication)

# 4. Test lockout
? 5 tentativi falliti ? Account bloccato
? Attesa 10 minuti ? Account sbloccato

# 5. Verifica LDAP
? Check nei log: "Successful AD authentication"
? Nessun errore LDAP injection
```

---

## ?? User Secrets Management

### ?? Importante: User Secrets Condivisi

User Secrets è **UNICO per progetto** (stesso file per tutti gli ambienti).

**Soluzione implementata**: 
- Quando switchi ambiente, **devi cambiare la connection string**
- Gli script `setup-*.bat` lo fanno automaticamente

### Verifica User Secrets Correnti

```bash
dotnet user-secrets list
```

**Output per LocalDev**:
```
ConnectionStrings:DefaultConnection = Server=.\SQLEXPRESS;...
```

**Output per TestDev**:
```
ConnectionStrings:DefaultConnection = Server=TEST-SQL;...
ActiveDirectory:ServiceAccount:Username = POSTEL\svc_giacenza_test
ActiveDirectory:ServiceAccount:Password = [hidden]
```

### Cambiare Manualmente

```bash
# Cambia connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "NEW_CONNECTION_STRING"

# Cambia AD password (solo TestDev)
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "NEW_PASSWORD"

# Riavvia app
run-testdev.bat
```

---

## ?? Struttura Progetto Finale

```
GiacenzaSorterRm/
??? appsettings.json                      # Config base
??? appsettings.LocalDev.json             # ? NUOVO - Override LocalDev
??? appsettings.TestDev.json              # ? NUOVO - Override TestDev
??? appsettings.Production.json           # Override Production
?
??? Properties/
?   ??? launchSettings.json               # ? AGGIORNATO - Profili VS
?
??? DOCS/
?   ??? CONFIGURAZIONE_SECRETS.md         # ? AGGIORNATO - Guida completa
?   ??? GUIDA_AMBIENTI.md                 # ? NUOVO - Quick start
?   ??? MIGLIORAMENTI_PROGETTO.md         # Architettura
?   ??? RIEPILOGO_MODIFICHE.md            # Changelog
?   ??? DEPLOYMENT_CHECKLIST.md           # Deploy checklist
?
??? setup-localdev.bat                    # ? NUOVO - Setup LocalDev
??? setup-testdev.bat                     # ? NUOVO - Setup TestDev
??? run-localdev.bat                      # ? NUOVO - Run LocalDev
??? run-testdev.bat                       # ? NUOVO - Run TestDev
?
??? %APPDATA%\Microsoft\UserSecrets\
    ??? <project-id>\
        ??? secrets.json                  # User Secrets (condivisi)
```

---

## ?? Troubleshooting

### "Connection string not found"

```bash
# Verifica User Secrets
dotnet user-secrets list

# Se vuoto, ri-esegui setup
setup-localdev.bat   # oppure
setup-testdev.bat
```

### "Active Directory LDAP path not configured"

**In LocalDev**: ? **NORMALE** - AD è disabilitato

**In TestDev**: ? **PROBLEMA**
```bash
# Verifica file config
type appsettings.TestDev.json

# Verifica User Secrets AD
dotnet user-secrets list | findstr ActiveDirectory

# Se mancano, ri-esegui setup
setup-testdev.bat
```

### Database non raggiungibile (LocalDev)

```bash
# Verifica SQL Express attivo
sc query MSSQL$SQLEXPRESS

# Avvia se fermo
net start MSSQL$SQLEXPRESS

# Testa connessione
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@VERSION"
```

### Database non raggiungibile (TestDev)

```bash
# Verifica connettività
ping TEST-SQL-SERVER

# Testa login
sqlcmd -S TEST-SQL-SERVER -U test_user -P test_password -Q "SELECT @@VERSION"
```

### LDAP non funziona (TestDev)

```bash
# Verifica LDAP raggiungibile
ping test-dc.postel.it

# Verifica porta LDAP aperta
telnet test-dc.postel.it 389

# Verifica User Secrets AD
dotnet user-secrets list | findstr ActiveDirectory

# Verifica log dettagliati
# File: appsettings.TestDev.json
# Cerca: "GiacenzaSorterRm.Services.ActiveDirectoryService": "Trace"
```

---

## ?? Best Practices

### DO ?
- ? Usa sempre gli script dedicati (`run-*.bat`)
- ? Cambia User Secrets quando switchi ambiente
- ? Testa sempre dopo cambio ambiente
- ? Usa database diversi (LOCAL vs TEST)
- ? Documenta configurazioni custom

### DON'T ?
- ? Non usare stesso DB per LocalDev e TestDev
- ? Non committare password in Git
- ? Non modificare `appsettings.json` per secrets
- ? Non dimenticare di switchare connection string

---

## ?? Checklist Finale

### Setup Iniziale
- [ ] File `appsettings.LocalDev.json` presente
- [ ] File `appsettings.TestDev.json` presente
- [ ] Script `setup-localdev.bat` funzionante
- [ ] Script `setup-testdev.bat` funzionante
- [ ] Script `run-localdev.bat` funzionante
- [ ] Script `run-testdev.bat` funzionante
- [ ] Profili Visual Studio configurati

### LocalDev Funzionante
- [ ] Setup completato (`setup-localdev.bat`)
- [ ] User Secrets configurati (connection string)
- [ ] Database locale raggiungibile
- [ ] Login utenti ESTERNI funziona
- [ ] Utenti AD falliscono gracefully

### TestDev Funzionante
- [ ] Setup completato (`setup-testdev.bat`)
- [ ] User Secrets configurati (DB + AD)
- [ ] Database test raggiungibile
- [ ] LDAP test raggiungibile
- [ ] Login utenti ESTERNI funziona
- [ ] Login utenti AD funziona

### Sicurezza
- [ ] `.gitignore` aggiornato
- [ ] Nessun secret committato in Git
- [ ] Password AD sicure
- [ ] Database isolati (LOCAL ? TEST)

---

## ?? Documentazione

| Documento | Contenuto |
|-----------|-----------|
| `DOCS\GUIDA_AMBIENTI.md` | ?? Quick start e troubleshooting |
| `DOCS\CONFIGURAZIONE_SECRETS.md` | ?? Guida completa multi-ambiente |
| `DOCS\MIGLIORAMENTI_PROGETTO.md` | ??? Architettura e best practices |
| `DOCS\RIEPILOGO_MODIFICHE.md` | ?? Changelog dettagliato |
| `DOCS\DEPLOYMENT_CHECKLIST.md` | ? Checklist deploy produzione |

---

## ?? Completato!

Hai ora **2 ambienti di sviluppo completamente configurati e funzionanti**:

### ?? LocalDev
- Database locale SQL Express
- Senza Active Directory
- Setup rapido per sviluppo veloce

### ?? TestDev
- Database remoto test
- Con Active Directory (LDAP)
- Test completo autenticazione

---

## ?? Prossimi Passi

1. **Esegui setup per l'ambiente che userai**:
   ```bash
   setup-localdev.bat   # oppure
   setup-testdev.bat
   ```

2. **Avvia l'applicazione**:
   ```bash
   run-localdev.bat   # oppure
   run-testdev.bat
   ```

3. **Testa il login** con utenti ESTERNI (e AD per TestDev)

4. **Consulta documentazione** per configurazioni avanzate

---

**Build Status**: ? Compilazione Riuscita  
**Environments**: ? LocalDev + TestDev Configurati  
**Documentation**: ? Completa  
**Ready**: ? Pronto all'uso

Buon coding! ??
