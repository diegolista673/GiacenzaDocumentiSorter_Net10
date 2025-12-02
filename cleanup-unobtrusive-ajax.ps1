# Script per rimuovere include inutili di jquery.unobtrusive-ajax.js
Write-Host "=== Cleanup jquery.unobtrusive-ajax.js ===" -ForegroundColor Cyan
Write-Host ""

$files = @(
    "Pages\PagesAssociazione\Index.cshtml",
    "Pages\PagesOperatori\Index.cshtml",
    "Pages\TipiContenitori\Index.cshtml",
    "Pages\TipiDocumenti\Index.cshtml",
    "Pages\TipologiaNormalizzazione\Index.cshtml",
    "Pages\TipoPiattaforme\Index.cshtml"
)

$pattern = '.*<script src="~/lib/jquery-unobtrusive-ajax/jquery.unobtrusive-ajax.js"></script>.*'
$count = 0

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        if ($content -match $pattern) {
            # Rimuovi la linea
            $newContent = $content -replace $pattern, ''
            
            # Rimuovi linee vuote multiple
            $newContent = $newContent -replace '(\r?\n){3,}', "`r`n`r`n"
            
            Set-Content $file $newContent -NoNewline
            Write-Host "[OK] $file - include rimosso" -ForegroundColor Green
            $count++
        } else {
            Write-Host "[SKIP] $file - pattern non trovato" -ForegroundColor Gray
        }
    } else {
        Write-Host "[WARN] $file - file non trovato" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File puliti: $count" -ForegroundColor Green
Write-Host ""
Write-Host "Verifica finale:" -ForegroundColor Yellow
Write-Host "dotnet build" -ForegroundColor White
Write-Host ""
