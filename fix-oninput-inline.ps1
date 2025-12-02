# Script per rimuovere oninput inline e sostituire con data-transform
Write-Host "=== Fix oninput Inline ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern: oninput="this.value = this.value.toUpperCase()"
    if ($content -match 'oninput="this\.value\s*=\s*this\.value\.toUpperCase\(\)"') {
        # Rimuovi oninput e aggiungi data-transform se non già presente
        $content = $content -replace 'oninput="this\.value\s*=\s*this\.value\.toUpperCase\(\)"\s*', ''
        
        # Aggiungi data-transform agli input che non lo hanno già
        $content = $content -replace '(<input[^>]*class="[^"]*input-dispaccio[^"]*"(?![^>]*data-transform)[^>]*)(>|/>)', '$1 data-transform="uppercase"$2'
        $content = $content -replace '(<input[^>]*class="[^"]*input-scatola[^"]*"(?![^>]*data-transform)[^>]*)(>|/>)', '$1 data-transform="uppercase"$2'
        $content = $content -replace '(<input[^>]*placeholder="[^"]*[Ss]catola[^"]*"(?![^>]*data-transform)[^>]*)(>|/>)', '$1 data-transform="uppercase"$2'
        $content = $content -replace '(<input[^>]*placeholder="[^"]*[Dd]ispaccio[^"]*"(?![^>]*data-transform)[^>]*)(>|/>)', '$1 data-transform="uppercase"$2'
        
        $modified = $true
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - oninput rimosso e sostituito con data-transform" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
