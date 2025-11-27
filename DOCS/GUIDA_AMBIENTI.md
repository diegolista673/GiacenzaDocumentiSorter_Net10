# ?? Guida Rapida - Ambienti di Sviluppo

## ? File Creati

### Configurazioni Ambiente
- ? `appsettings.LocalDev.json` - Config ambiente senza LDAP
- ? `appsettings.TestDev.json` - Config ambiente con LDAP

### Script di Setup
- ? `setup-localdev.bat` - Setup guidato LocalDev
- ? `setup-testdev.bat` - Setup guidato TestDev

### Script di Avvio
- ? `run-localdev.bat` - Avvia in LocalDev
- ? `run-testdev.bat` - Avvia in TestDev

### Profili Visual Studio
- ? `Properties\launchSettings.json` - Aggiornato con profili LocalDev/TestDev

---

## ?? Come Iniziare

### Prima Volta - Ambiente LocalDev (Senza LDAP)

```bash
# 1. Naviga nel progetto
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm

# 2. Esegui setup
setup-localdev.bat

# 3. Segui wizard interattivo
#    - Scegli opzione database
#    - Inserisci parametri richiesti

# 4. Avvia applicazione
run-localdev.bat
```

### Prima Volta - Ambiente TestDev (Con LDAP)

```bash
# 1. Naviga nel progetto
cd C:\Users\SMARTW\Desktop\GiacenzaSorterRm

# 2. Esegui setup
setup-testdev.bat

# 3. Segui wizard interattivo
#    - Inserisci server DB test
#    - Inserisci credenziali AD service account

# 4. Avvia applicazione
run-testdev.bat
```

---

## ?? Switching Tra Ambienti

### Metodo 1: Script (CONSIGLIATO)

```bash
# LocalDev (DB locale, no LDAP)
run-localdev.bat

# TestDev (DB test, con LDAP)
run-testdev.bat
```

### Metodo 2: Visual Studio

1. Apri Visual Studio
2. Toolbar in alto: **Debug dropdown**
3. Seleziona profilo:
   - "LocalDev (Senza LDAP)"
   - "TestDev (Con LDAP)"
4. Premi F5

![Visual Studio Profiles](https://i.imgur.com/placeholder.png)

### Metodo 3: Command Line

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

### LocalDev
| Setting | Valore |
|---------|--------|
| **Database** | SQL Express Locale |
| **Active Directory** | ? Disabilitato |
| **Utenti Supportati** | Solo ESTERNI (password hash) |
| **Max Failed Attempts** | 10 tentativi |
| **Lockout Duration** | 5 minuti |
| **Log Level** | Debug |

**Connection String Esempio:**
```
Server=.\SQLEXPRESS;Database=GIACENZA_SORTER_RM_LOCAL;Integrated Security=True;TrustServerCertificate=True;
```

### TestDev
| Setting | Valore |
|---------|--------|
| **Database** | Server Test Remoto |
| **Active Directory** | ? Abilitato (test-dc.postel.it) |
| **Utenti Supportati** | ESTERNI + AD |
| **Max Failed Attempts** | 5 tentativi |
| **Lockout Duration** | 10 minuti |
| **Log Level** | Debug + Trace (AD) |

**Connection String Esempio:**
```
Server=TEST-SQL-SERVER;Database=GIACENZA_SORTER_RM_TEST;User Id=test_user;Password=test_password;TrustServerCertificate=True;
```

**Service Account:**
```
Username: POSTEL\svc_giacenza_test
Password: [configurato in User Secrets]
```

---

## ?? Testing

### Test LocalDev

1. **Avvia**: `run-localdev.bat`
2. **Verifica log**:
   ```
   Hosting environment: LocalDev
   Active Directory: DISABILITATO
   ```
3. **Test login**:
   - ? Utente ESTERNO ? Funziona
   - ? Utente AD ? Fallisce (atteso)

### Test TestDev

1. **Avvia**: `run-testdev.bat`
2. **Verifica log**:
   ```
   Hosting environment: TestDev
   Active Directory: ABILITATO
   LDAP Path: LDAP://test-dc.postel.it
   ```
3. **Test login**:
   - ? Utente ESTERNO ? Funziona
   - ? Utente AD ? Funziona (LDAP)

---

## ?? Modificare Configurazioni

### Cambiare Connection String

```bash
# Per LocalDev
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "NEW_CONNECTION_STRING"

# Riavvia applicazione
run-localdev.bat
```

### Cambiare Password AD (TestDev)

```bash
dotnet user-secrets set "ActiveDirectory:ServiceAccount:Password" "NEW_PASSWORD"

# Riavvia applicazione
run-testdev.bat
```

### Verificare Configurazione Corrente

```bash
# Mostra tutti i secrets
dotnet user-secrets list

# Mostra ambiente attivo
echo %ASPNETCORE_ENVIRONMENT%
```

---

## ?? Problemi Comuni

### "Connection string not found"

**Causa**: User Secrets non configurati

**Soluzione**:
```bash
# Ri-esegui setup per l'ambiente
setup-localdev.bat   # oppure
setup-testdev.bat
```

### "Active Directory LDAP path not configured" (in TestDev)

**Causa**: Service account non configurato

**Soluzione**:
```bash
setup-testdev.bat
```

### Database non raggiungibile

**LocalDev**:
```bash
# Verifica SQL Express attivo
sc query MSSQL$SQLEXPRESS

# Avvia se fermato
net start MSSQL$SQLEXPRESS
```

**TestDev**:
```bash
# Verifica connettività
ping TEST-SQL-SERVER

# Testa login
sqlcmd -S TEST-SQL-SERVER -U test_user -P test_password -Q "SELECT 1"
```

### Utenti AD non funzionano in TestDev

**Verifica**:
1. Service account configurato?
   ```bash
   dotnet user-secrets list | findstr ActiveDirectory
   ```

2. LDAP raggiungibile?
   ```bash
   ping test-dc.postel.it
   ```

3. Log dettagliati abilitati:
   - File: `appsettings.TestDev.json`
   - Sezione: `"GiacenzaSorterRm.Services.ActiveDirectoryService": "Trace"`

---

## ?? Comparazione Ambienti

| Feature | LocalDev | TestDev | Production |
|---------|----------|---------|------------|
| Database | Locale | Test Remoto | Produzione |
| Active Directory | ? No | ? Test | ? Prod |
| Utenti ESTERNI | ? | ? | ? |
| Utenti AD | ? | ? | ? |
| Max Failed Attempts | 10 | 5 | 5 |
| Lockout Minutes | 5 | 10 | 15 |
| Log Level | Debug | Debug+Trace | Warning |
| Configuration | User Secrets | User Secrets | Env Vars |

---

## ?? Checklist Prima di Committare

- [ ] **NON** committare `secrets.*.json` files
- [ ] Verificare `.gitignore` aggiornato
- [ ] Testare sia LocalDev che TestDev
- [ ] Documentare eventuali nuove configurazioni
- [ ] Verificare che script `.bat` funzionino

---

## ?? Best Practices

### DO ?
- ? Usa script dedicati per ogni ambiente
- ? Testa sempre dopo cambio ambiente
- ? Documenta configurazioni custom
- ? Usa database diversi per LocalDev e TestDev
- ? Mantieni User Secrets aggiornati

### DON'T ?
- ? Non committare password in Git
- ? Non usare stesso DB per dev e test
- ? Non modificare `appsettings.json` per secrets
- ? Non dimenticare di switchare User Secrets

---

## ?? Supporto

### Documentazione
- `DOCS\CONFIGURAZIONE_SECRETS.md` - Guida completa
- `DOCS\MIGLIORAMENTI_PROGETTO.md` - Architettura
- `DOCS\RIEPILOGO_MODIFICHE.md` - Changelog

### Quick Help
```bash
# Mostra ambiente corrente
echo %ASPNETCORE_ENVIRONMENT%

# Mostra User Secrets
dotnet user-secrets list

# Re-setup ambiente
setup-localdev.bat   # o setup-testdev.bat

# Verifica connessione DB
sqlcmd -S [SERVER] -U [USER] -P [PASSWORD] -Q "SELECT 1"
```

---

## ?? Tutto Pronto!

Ora hai **2 ambienti di sviluppo completamente configurati**:

1. **LocalDev** - Sviluppo rapido locale senza dipendenze LDAP
2. **TestDev** - Test completo con Active Directory

Per iniziare:
```bash
setup-localdev.bat
run-localdev.bat
```

Buon coding! ??
