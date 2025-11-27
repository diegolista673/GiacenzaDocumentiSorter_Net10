# ?? Migrazione Database SQL Server ? SQLite

Guida completa per creare una copia locale SQLite del database SQL Server.

---

## ?? Perché SQLite?

? **Veloce** - Database file-based senza server  
? **Portable** - Un singolo file `.db`  
? **Zero Config** - Nessuna installazione SQL Server necessaria  
? **Sviluppo Offline** - Lavora senza connessione al server  

---

## ?? Prerequisiti

### 1. Modulo PowerShell SqlServer

```powershell
Install-Module -Name SqlServer -Scope CurrentUser
```

### 2. Packages NuGet (già inclusi)

- `Microsoft.EntityFrameworkCore.Sqlite` - Provider EF Core per SQLite
- `CsvHelper` - Per import/export CSV
- `BCrypt.Net-Next` - Per password hashing

---

## ?? Migrazione Completa (Automatica)

### Step 1: Esporta Dati da SQL Server

```powershell
.\migrate-sqlserver-to-sqlite.ps1
```

Questo script:
1. ? Esporta tutte le tabelle da SQL Server in CSV
2. ? Crea database SQLite vuoto con schema corretto
3. ? Prepara file CSV in `.\Data\Export\`

### Step 2: Importa Dati in SQLite

```powershell
.\import-data-to-sqlite.ps1
```

Oppure manualmente con **DB Browser for SQLite**:
1. Download: https://sqlitebrowser.org/
2. Apri: `giacenza_sorter_local.db`
3. File ? Import ? Table from CSV file
4. Importa ogni file CSV nella tabella corrispondente

---

## ?? Ordine Importazione Tabelle

?? **IMPORTANTE**: Importa nell'ordine per rispettare le foreign keys:

1. `CentriLav`
2. `Stati`
3. `Operatori`
4. `Piattaforme`
5. `Commesse`
6. `Tipologie`
7. `Contenitori`
8. `TipiNormalizzazione`
9. `CommessaTipologiaContenitore`
10. `Bancali`
11. `BancaliDispacci`
12. `Scatole`

---

## ?? Configurazione Applicazione

### Usare SQLite invece di SQL Server

Modifica `appsettings.LocalDev.json`:

```json
{
  "UseSQLite": true,
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=giacenza_sorter_local.db"
  }
}
```

### Avvia Applicazione

```powershell
dotnet run --environment LocalDev
```

L'applicazione userà automaticamente SQLite se `UseSQLite: true`.

---

## ?? Struttura File

```
GiacenzaSorterRm/
??? giacenza_sorter_local.db          # Database SQLite
??? giacenza_sorter_local.db-shm      # Shared memory (auto)
??? giacenza_sorter_local.db-wal      # Write-ahead log (auto)
??? Data/
?   ??? Export/                       # CSV esportati da SQL Server
?   ?   ??? CentriLav.csv
?   ?   ??? Stati.csv
?   ?   ??? ...
?   ??? GiacenzaSorterSqliteContext.cs
?   ??? SqliteDataImporter.cs
??? Migrations/
?   ??? SQLite/                       # Migrations specifiche SQLite
??? migrate-sqlserver-to-sqlite.ps1   # Script esportazione
??? import-data-to-sqlite.ps1         # Script importazione
```

---

## ?? Aggiornamento Dati

Per aggiornare il database SQLite con nuovi dati da SQL Server:

```powershell
# 1. Elimina database SQLite vecchio
Remove-Item giacenza_sorter_local.db -Force

# 2. Ri-esegui migrazione
.\migrate-sqlserver-to-sqlite.ps1

# 3. Importa dati aggiornati
.\import-data-to-sqlite.ps1
```

---

## ?? Troubleshooting

### Problema: "Foreign key constraint failed"

**Causa**: Importazione non nell'ordine corretto  
**Soluzione**: Elimina database e ri-importa nell'ordine specificato

```powershell
Remove-Item giacenza_sorter_local.db -Force
.\migrate-sqlserver-to-sqlite.ps1
```

### Problema: "Table already exists"

**Causa**: Database non eliminato prima di ricreare  
**Soluzione**:

```powershell
Remove-Item giacenza_sorter_local.db* -Force
dotnet ef database update --context GiacenzaSorterSqliteContext
```

### Problema: "Cannot open database file"

**Causa**: File database in uso da altro processo  
**Soluzione**: Chiudi Visual Studio e altri tool che accedono al DB

```powershell
# Termina processi bloccanti
Get-Process | Where-Object {$_.ProcessName -like "*sqlite*"} | Stop-Process -Force
```

---

## ?? Differenze SQL Server vs SQLite

| Caratteristica | SQL Server | SQLite |
|----------------|------------|--------|
| **Tipo Dati** | Molti tipi nativi | 5 tipi base (INTEGER, TEXT, REAL, BLOB, NULL) |
| **Auto Increment** | `IDENTITY(1,1)` | `AUTOINCREMENT` su INTEGER PRIMARY KEY |
| **DateTime** | `datetime`, `datetime2` | `TEXT` (ISO8601) o `INTEGER` (Unix) |
| **Boolean** | `bit` | `INTEGER` (0/1) |
| **Varchar** | `varchar(n)`, `nvarchar(n)` | `TEXT` |
| **Foreign Keys** | Sempre attive | Vanno abilitate: `PRAGMA foreign_keys = ON` |
| **Schema** | `[dbo].[TableName]` | Solo `TableName` |

---

## ?? Verifica Database SQLite

### Via Command Line

```powershell
# Installa sqlite3 (Windows)
# Download: https://www.sqlite.org/download.html

# Verifica tabelle
sqlite3 giacenza_sorter_local.db ".tables"

# Conta righe
sqlite3 giacenza_sorter_local.db "SELECT COUNT(*) FROM Scatole"

# Schema tabella
sqlite3 giacenza_sorter_local.db ".schema Scatole"
```

### Via DB Browser for SQLite (GUI)

1. Download: https://sqlitebrowser.org/
2. Apri `giacenza_sorter_local.db`
3. Browse Data ? Seleziona tabella
4. Execute SQL ? Query personalizzate

---

## ?? Best Practices

### DO ?

- ? Usa SQLite **solo per sviluppo locale**
- ? Esporta dati regolarmente da SQL Server
- ? Testa su SQL Server prima di deployare
- ? Aggiungi `*.db` a `.gitignore`

### DON'T ?

- ? Non usare SQLite in produzione per questo progetto
- ? Non committare file `.db` in Git
- ? Non fare modifiche schema solo su SQLite
- ? Non aspettarti performance identiche a SQL Server

---

## ?? Risorse

- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [EF Core SQLite Provider](https://docs.microsoft.com/en-us/ef/core/providers/sqlite/)
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- [SQLite vs SQL Server](https://www.sqlite.org/whentouse.html)

---

## ?? Note

- Il database SQLite è **read-write** completo
- Tutte le operazioni CRUD funzionano
- Active Directory è **mockato** in LocalDev
- Performance eccellenti per database < 100MB

---

**Ultima modifica**: 2025-01-24  
**Versione**: 1.0  
**Autore**: GiacenzaSorterRm Team
