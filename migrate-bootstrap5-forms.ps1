# Script per Migrazione Form da Bootstrap 4 a Bootstrap 5
# Autore: GitHub Copilot
# Data: 2025-01-24

Write-Host "=== Migrazione Form Bootstrap 4 ? Bootstrap 5 ===" -ForegroundColor Cyan
Write-Host ""

# Pattern da sostituire
$replacements = @{
    # form-group ? mb-3 (margin-bottom: 1rem)
    'class="form-group"' = 'class="mb-3"'
    "class='form-group'" = "class='mb-3'"
    
    # form-row ? row g-3 (gutter spacing)
    'class="form-row"' = 'class="row g-3"'
    "class='form-row'" = "class='row g-3'"
    
    # input-group-prepend/append ? rimossi (non necessari in BS5)
    '<div class="input-group-prepend">' = ''
    '<div class="input-group-append">' = ''
    '</div><!-- input-group-prepend -->' = ''
    '</div><!-- input-group-append -->' = ''
    
    # form-inline ? d-flex (usando flexbox)
    'class="form-inline"' = 'class="d-flex flex-wrap gap-2"'
    "class='form-inline'" = "class='d-flex flex-wrap gap-2'"
    
    # custom-select ? form-select
    'class="custom-select"' = 'class="form-select"'
    "class='custom-select'" = "class='form-select'"
    
    # custom-file ? form-control (per file input)
    'class="custom-file-input"' = 'class="form-control"'
    'class="custom-file-label"' = ''
    
    # custom-range ? form-range
    'class="custom-range"' = 'class="form-range"'
    
    # custom-switch ? form-check form-switch
    'class="custom-switch"' = 'class="form-check form-switch"'
    'class="custom-control-input"' = 'class="form-check-input"'
    'class="custom-control-label"' = 'class="form-check-label"'
}

# Pattern regex per classi miste (es: class="form-group row")
$regexReplacements = @(
    @{
        Pattern = 'class="([^"]*\s)?form-group(\s[^"]*)?'
        Replacement = 'class="$1mb-3$2'
        Description = "form-group ? mb-3 (in classi miste)"
    },
    @{
        Pattern = 'class="([^"]*\s)?form-row(\s[^"]*)?'
        Replacement = 'class="$1row g-3$2'
        Description = "form-row ? row g-3 (in classi miste)"
    },
    @{
        Pattern = 'class="([^"]*\s)?custom-select(\s[^"]*)?'
        Replacement = 'class="$1form-select$2'
        Description = "custom-select ? form-select (in classi miste)"
    },
    @{
        Pattern = 'class="([^"]*\s)?form-inline(\s[^"]*)?'
        Replacement = 'class="$1d-flex flex-wrap gap-2$2'
        Description = "form-inline ? d-flex (in classi miste)"
    }
)

# Trova tutti i file .cshtml (escludi _Layout e partials se necessario)
$files = Get-ChildItem -Path "Pages" -Filter "*.cshtml" -Recurse | 
         Where-Object { 
             $_.Name -notlike "_*" -and 
             $_.Directory.Name -notlike "Shared"
         }

Write-Host "Trovati $($files.Count) file da processare" -ForegroundColor Yellow
Write-Host ""

$totalModified = 0
$totalReplacements = 0
$modifiedFiles = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileReplacements = 0
    
    # Sostituzioni semplici
    foreach ($key in $replacements.Keys) {
        $value = $replacements[$key]
        if ($content -like "*$key*") {
            $count = ([regex]::Matches($content, [regex]::Escape($key))).Count
            $content = $content -replace [regex]::Escape($key), $value
            $fileReplacements += $count
        }
    }
    
    # Sostituzioni regex
    foreach ($regexReplacement in $regexReplacements) {
        $matches = [regex]::Matches($content, $regexReplacement.Pattern)
        if ($matches.Count -gt 0) {
            $content = $content -replace $regexReplacement.Pattern, $regexReplacement.Replacement
            $fileReplacements += $matches.Count
        }
    }
    
    # Se ci sono state modifiche, salva il file
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline -Encoding UTF8
        
        $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
        Write-Host "? $relativePath" -ForegroundColor Green
        Write-Host "  Sostituzioni: $fileReplacements" -ForegroundColor Gray
        
        $totalModified++
        $totalReplacements += $fileReplacements
        $modifiedFiles += $relativePath
    }
}

Write-Host ""
Write-Host "=== Riepilogo ===" -ForegroundColor Cyan
Write-Host "File processati: $($files.Count)" -ForegroundColor White
Write-Host "File modificati: $totalModified" -ForegroundColor Green
Write-Host "Sostituzioni totali: $totalReplacements" -ForegroundColor Green

if ($totalModified -gt 0) {
    Write-Host ""
    Write-Host "File modificati:" -ForegroundColor Cyan
    foreach ($file in $modifiedFiles) {
        Write-Host "  - $file" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "? IMPORTANTE: Verifica modifiche prima di committare!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Prossimi passi:" -ForegroundColor Cyan
    Write-Host "1. Ricompila progetto: dotnet build" -ForegroundColor White
    Write-Host "2. Testa visivamente ogni form" -ForegroundColor White
    Write-Host "3. Verifica spaziatura e layout" -ForegroundColor White
    Write-Host "4. Commit: git add . && git commit -m 'fix: Migrato form da Bootstrap 4 a Bootstrap 5'" -ForegroundColor White
}
else {
    Write-Host ""
    Write-Host "? Nessuna modifica necessaria" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Script Completato ===" -ForegroundColor Cyan
