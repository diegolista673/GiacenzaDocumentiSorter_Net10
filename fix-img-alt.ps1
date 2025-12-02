# Script per aggiungere alt attribute a tutte le immagini Spinner.gif
Write-Host "=== Fix IMG Alt Attributes ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern 1: <img src="~/images/Spinner.gif" />
    if ($content -match '<img\s+src="~/images/Spinner\.gif"\s*/>') {
        $content = $content -replace '<img\s+src="~/images/Spinner\.gif"\s*/>', '<img src="~/images/Spinner.gif" alt="Caricamento in corso..." />'
        $modified = $true
    }
    
    # Pattern 2: <img class="..." src="~/images/Spinner.gif" />
    if ($content -match '<img\s+class="([^"]+)"\s+src="~/images/Spinner\.gif"\s*/>') {
        $content = $content -replace '<img\s+class="([^"]+)"\s+src="~/images/Spinner\.gif"\s*/>', '<img class="$1" src="~/images/Spinner.gif" alt="Caricamento in corso..." />'
        $modified = $true
    }
    
    # Pattern 3: <img src="~/images/Spinner.gif" > (senza self-closing)
    if ($content -match '<img\s+src="~/images/Spinner\.gif"\s*>') {
        $content = $content -replace '<img\s+src="~/images/Spinner\.gif"\s*>', '<img src="~/images/Spinner.gif" alt="Caricamento in corso..." />'
        $modified = $true
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - alt attribute aggiunto" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
