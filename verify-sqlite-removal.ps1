# Script di verifica rimozione SQLite
Write-Host "=== Verifica Rimozione SQLite dal Progetto ===" -ForegroundColor Cyan
Write-Host ""

$errors = 0

# 1. Verifica file SQLite eliminati
Write-Host "1. Verifica file SQLite eliminati..." -ForegroundColor Yellow

$sqliteFiles = @(
    "Data\GiacenzaSorterSqliteContext.cs",
    "Data\SqliteDataImporter.cs",
    "DOCS\MIGRAZIONE_SQLITE.md",
    "DOCS\FIX_DATETIME_SQLITE.md",
    "Migrations\SQLite"
)

foreach ($file in $sqliteFiles) {
    if (Test-Path $file) {
        Write-Host "  [ERRORE] File ancora presente: $file" -ForegroundColor Red
        $errors++
    } else {
        Write-Host "  [OK] File rimosso: $file" -ForegroundColor Green
    }
}

# 2. Verifica riferimenti SQLite nel codice
Write-Host ""
Write-Host "2. Verifica riferimenti SQLite nel codice..." -ForegroundColor Yellow

$searchPatterns = @(
    "GiacenzaSorterSqliteContext",
    "UseSqlite\s*\(",
    "\.UseSqlite",
    "UseSQLite",
    "SQLiteConnection"
)

$sourceFiles = Get-ChildItem -Recurse -Include *.cs,*.json,*.cshtml -Exclude bin,obj,node_modules

foreach ($pattern in $searchPatterns) {
    $matches = $sourceFiles | Select-String -Pattern $pattern
    
    if ($matches) {
        Write-Host "  [WARN] Riferimenti trovati per pattern '$pattern':" -ForegroundColor Yellow
        foreach ($match in $matches) {
            Write-Host "    - $($match.Path):$($match.LineNumber)" -ForegroundColor Gray
            $errors++
        }
    }
}

if ($errors -eq 0) {
    Write-Host "  [OK] Nessun riferimento SQLite trovato nel codice" -ForegroundColor Green
}

# 3. Verifica Startup.cs
Write-Host ""
Write-Host "3. Verifica Startup.cs..." -ForegroundColor Yellow

$startupContent = Get-Content "Startup.cs" -Raw

if ($startupContent -match "GiacenzaSorterSqliteContext") {
    Write-Host "  [ERRORE] Riferimento a GiacenzaSorterSqliteContext trovato" -ForegroundColor Red
    $errors++
} elseif ($startupContent -match "useSqlite") {
    Write-Host "  [ERRORE] Variabile useSqlite trovata" -ForegroundColor Red
    $errors++
} else {
    Write-Host "  [OK] Startup.cs pulito" -ForegroundColor Green
}

# 4. Verifica appsettings
Write-Host ""
Write-Host "4. Verifica appsettings.*.json..." -ForegroundColor Yellow

$appsettingsFiles = Get-ChildItem -Filter "appsettings*.json"

foreach ($file in $appsettingsFiles) {
    $content = Get-Content $file.FullName -Raw
    
    if ($content -match "UseSQLite") {
        Write-Host "  [ERRORE] Proprietà UseSQLite trovata in $($file.Name)" -ForegroundColor Red
        $errors++
    } elseif ($content -match "SQLiteConnection") {
        Write-Host "  [ERRORE] Proprietà SQLiteConnection trovata in $($file.Name)" -ForegroundColor Red
        $errors++
    } else {
        Write-Host "  [OK] $($file.Name) pulito" -ForegroundColor Green
    }
}

# 5. Verifica IAppDbContext ancora presente
Write-Host ""
Write-Host "5. Verifica IAppDbContext..." -ForegroundColor Yellow

if (Test-Path "Data\IAppDbContext.cs") {
    Write-Host "  [OK] IAppDbContext presente (necessaria per DI)" -ForegroundColor Green
} else {
    Write-Host "  [ERRORE] IAppDbContext mancante!" -ForegroundColor Red
    $errors++
}

# 6. Verifica GiacenzaSorterRmTestContext implementa IAppDbContext
Write-Host ""
Write-Host "6. Verifica implementazione IAppDbContext..." -ForegroundColor Yellow

$contextContent = Get-Content "Models\Database\GiacenzaSorterRmTestContext.cs" -Raw

if ($contextContent -match ":\s*DbContext,\s*GiacenzaSorterRm\.Data\.IAppDbContext") {
    Write-Host "  [OK] GiacenzaSorterRmTestContext implementa IAppDbContext" -ForegroundColor Green
} else {
    Write-Host "  [ERRORE] GiacenzaSorterRmTestContext non implementa IAppDbContext" -ForegroundColor Red
    $errors++
}

# 7. Verifica file database SQLite eliminati
Write-Host ""
Write-Host "7. Verifica file database SQLite..." -ForegroundColor Yellow

$dbFiles = Get-ChildItem -Filter "*.db*" -ErrorAction SilentlyContinue

if ($dbFiles) {
    Write-Host "  [WARN] File database SQLite trovati:" -ForegroundColor Yellow
    foreach ($dbFile in $dbFiles) {
        Write-Host "    - $($dbFile.Name)" -ForegroundColor Gray
    }
    Write-Host "  Esegui: Remove-Item *.db* -Force" -ForegroundColor Yellow
} else {
    Write-Host "  [OK] Nessun file database SQLite presente" -ForegroundColor Green
}

# Riepilogo
Write-Host ""
Write-Host "=== Riepilogo ===" -ForegroundColor Cyan

if ($errors -eq 0) {
    Write-Host "? SQLite rimosso completamente dal progetto!" -ForegroundColor Green
    Write-Host "? IAppDbContext mantenuta per Dependency Injection" -ForegroundColor Green
    Write-Host "? Progetto configurato per usare solo SQL Server/Azure SQL" -ForegroundColor Green
} else {
    Write-Host "? Trovati $errors problemi" -ForegroundColor Red
    Write-Host "Rivedi i messaggi sopra per i dettagli" -ForegroundColor Yellow
}

Write-Host ""
