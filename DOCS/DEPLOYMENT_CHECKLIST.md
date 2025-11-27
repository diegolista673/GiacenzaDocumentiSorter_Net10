# ?? Deployment Checklist - GiacenzaSorterRm

## Pre-Deployment

### Development Environment
- [ ] User Secrets configurati (`dotnet user-secrets list` per verificare)
- [ ] Applicazione testata localmente
- [ ] Login funzionante con utenti esterni
- [ ] Login funzionante con utenti AD (se service account configurato)
- [ ] Account lockout testato (5 tentativi falliti)
- [ ] File `MyConnections.cs` rimosso dal repository
- [ ] Build senza errori (`dotnet build`)
- [ ] Commit con messaggio: "feat: Security hardening - Authentication refactoring"

### Code Review
- [ ] Verificato che nessuna password sia committata
- [ ] Verificato `.gitignore` aggiornato
- [ ] Verificato `appsettings.json` contiene solo PLACEHOLDER
- [ ] Verificato che `web.config` non sia nel repository
- [ ] Review servizi `AuthenticationService` e `ActiveDirectoryService`

---

## Production Deployment

### 1. Backup Pre-Deploy
- [ ] Backup database `GIACENZA_SORTER_RM`
- [ ] Backup application files correnti
- [ ] Backup configurazioni IIS

### 2. Server Preparation
- [ ] .NET 8 Runtime installato
- [ ] IIS configurato per ASP.NET Core
- [ ] SQL Server accessibile
- [ ] Active Directory raggiungibile
- [ ] Firewall configurato (porte LDAP/LDAPS)

### 3. Service Account Setup
- [ ] Creato service account AD per query LDAP
  - Username: `POSTEL\svc_giacenza`
  - Permessi: Lettura oggetti user in AD
  - Password complessa
- [ ] Testato service account con query LDAP
- [ ] Documentata password in vault sicuro

### 4. IIS Configuration

#### Application Pool
- [ ] Creato Application Pool `GiacenzaSorterRmAppPool`
- [ ] .NET CLR Version: **No Managed Code**
- [ ] Identity: **ApplicationPoolIdentity**
- [ ] Start Mode: **AlwaysRunning**

#### Environment Variables (Application Pool)
Configurare in: Configuration Editor ? system.applicationHost/applicationPools

```
ConnectionStrings__DefaultConnection = Server=SRVR-000EDP02.postel.it;Database=GIACENZA_SORTER_RM;User Id=UserProduzioneGed;Password=[PASSWORD_PRODUZIONE];TrustServerCertificate=True;

ActiveDirectory__ServiceAccount__Username = POSTEL\svc_giacenza

ActiveDirectory__ServiceAccount__Password = [SERVICE_ACCOUNT_PASSWORD]

ActiveDirectory__LdapPath = LDAP://dc01.postel.it

ASPNETCORE_ENVIRONMENT = Production
```

- [ ] Variabili configurate
- [ ] Password sicure utilizzate
- [ ] Application Pool riavviato

#### Website
- [ ] Sito creato: `GiacenzaSorterRm`
- [ ] Physical Path: `C:\inetpub\wwwroot\GiacenzaSorterRm`
- [ ] Application Pool: `GiacenzaSorterRmAppPool`
- [ ] Binding configurato (HTTPS consigliato)

### 5. Application Deployment
- [ ] Files copiati in `C:\inetpub\wwwroot\GiacenzaSorterRm`
- [ ] Permessi cartella verificati (IIS_IUSRS read/execute)
- [ ] File `web.config` presente (generato da publish)
- [ ] Logs folder creato: `C:\inetpub\wwwroot\GiacenzaSorterRm\logs`

### 6. Database
- [ ] Database `GIACENZA_SORTER_RM` esistente
- [ ] Tabella `Operatoris` verificata
- [ ] User `UserProduzioneGed` ha permessi corretti
- [ ] Connection string testata

### 7. SSL/TLS (CONSIGLIATO)
- [ ] Certificato SSL installato
- [ ] HTTPS binding configurato
- [ ] HTTP redirect a HTTPS abilitato
- [ ] HSTS header configurato

---

## Post-Deployment Testing

### Smoke Tests
- [ ] Homepage carica correttamente
- [ ] Pagina login accessibile
- [ ] Browser detection funziona (redirect IE)

### Authentication Tests
- [ ] ? Login utente esterno (password hash DB) funziona
- [ ] ? Login utente AD funziona
- [ ] ? Login con password errata fallisce
- [ ] ? Login con username inesistente fallisce
- [ ] ? Account lockout dopo 5 tentativi funziona
- [ ] ? Messaggio errore generico (no user enumeration)
- [ ] ? Redirect a `/Home` dopo login

### Security Tests
- [ ] ? Tentativo SQL injection fallisce
- [ ] ? Tentativo LDAP injection fallisce
- [ ] ? Exception details non visibili all'utente
- [ ] ? Log strutturati in `logs\` folder
- [ ] ? Cookie `HttpOnly` e `Secure` settati
- [ ] ? HTTPS redirect funziona (se configurato)

### Logging Tests
- [ ] ? Login success loggato
- [ ] ? Login failure loggato con username
- [ ] ? Account lockout loggato
- [ ] ? AD authentication errors loggati
- [ ] ? Password NON loggate

---

## Monitoring Setup

### Application Insights (Opzionale)
- [ ] Application Insights configurato
- [ ] Instrumentation key in environment variables
- [ ] Telemetry attiva

### Health Checks
- [ ] Endpoint `/health` accessibile
- [ ] Database health check OK
- [ ] AD connectivity check OK

### Alerting
- [ ] Alert configurato per failed logins spike
- [ ] Alert configurato per application errors
- [ ] Alert configurato per performance degradation

---

## Rollback Plan

### In caso di problemi critici:

1. **Immediate Rollback**
   ```powershell
   # Stop Application Pool
   Stop-WebAppPool -Name "GiacenzaSorterRmAppPool"
   
   # Restore backup files
   Copy-Item "C:\Backup\GiacenzaSorterRm\*" "C:\inetpub\wwwroot\GiacenzaSorterRm\" -Recurse -Force
   
   # Start Application Pool
   Start-WebAppPool -Name "GiacenzaSorterRmAppPool"
   ```

2. **Restore Database** (se necessario)
   ```sql
   RESTORE DATABASE [GIACENZA_SORTER_RM] 
   FROM DISK = 'C:\Backup\GIACENZA_SORTER_RM_PRE_DEPLOY.bak'
   WITH REPLACE;
   ```

3. **Notify Team**
   - Invia notifica team di sviluppo
   - Documenta issue riscontrato
   - Prepara fix per re-deploy

---

## Post-Deployment Documentation

### Da Aggiornare
- [ ] Documentazione operativa
- [ ] Runbook di troubleshooting
- [ ] Credenziali in password vault
- [ ] Diagramma architettura
- [ ] Processo di deploy

### Da Comunicare
- [ ] Team di sviluppo: Deploy completato
- [ ] Team Operations: Nuova configurazione
- [ ] Team Security: Vulnerabilità risolte
- [ ] Utenti: Eventuali downtime programmati

---

## Success Criteria

? Deployment considerato riuscito se:
- [ ] Applicazione raggiungibile
- [ ] Login funzionante (esterni + AD)
- [ ] Nessun errore critico nei log
- [ ] Performance accettabile
- [ ] Security tests passati
- [ ] Monitoring attivo
- [ ] Team notificato

---

## Emergency Contacts

| Ruolo | Nome | Contatto |
|-------|------|----------|
| Dev Team Lead | [NOME] | [EMAIL/PHONE] |
| DBA | [NOME] | [EMAIL/PHONE] |
| SysAdmin | [NOME] | [EMAIL/PHONE] |
| Security Team | [NOME] | [EMAIL/PHONE] |

---

## Notes / Issues

```
Data Deploy: _________________
Deploy By: ___________________
Issues Riscontrati:


Soluzioni Applicate:


Post-Deploy Actions:


```

---

**Checklist Version**: 1.0  
**Last Updated**: 2025-01-XX  
**Next Review**: Post primo deploy
