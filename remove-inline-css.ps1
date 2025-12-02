# Script per rimuovere CSS inline comuni e sostituire con classi
Write-Host "=== Rimozione CSS Inline ===" -ForegroundColor Cyan
Write-Host ""

$replacements = @{
    'style="max-width: 100%;"' = 'class="max-w-100"'
    'style="max-width: 100%"' = 'class="max-w-100"'
    'style="width:100%"' = 'class="w-100"'
    'style="width: 100%"' = 'class="w-100"'
    'style="display:none;"' = 'class="d-none"'
    'style="display: none;"' = 'class="d-none"'
}

$files = @(
    "Pages\PagesNormalizzato\Index.cshtml",
    "Pages\PagesSorter\Create.cshtml",
    "Pages\PagesAccettazione\Create.cshtml",
    "Pages\PagesRicercaDispaccio\Index.cshtml",
    "Pages\PagesRiepilogo\Index.cshtml",
    "Pages\PagesRiepilogoBancali\Index.cshtml",
    "Pages\PagesSorterizzato\Index.cshtml",
    "Pages\PagesVolumi\Index.cshtml",
    "Pages\PagesSpostaGiacenza\Create.cshtml"
)

$count = 0

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $modified = $false
        
        foreach ($pattern in $replacements.Keys) {
            if ($content -match [regex]::Escape($pattern)) {
                $replacement = $replacements[$pattern]
                
                # Se l'elemento ha già una classe, aggiungi la nuova classe
                if ($content -match "$([regex]::Escape($pattern))[^>]*class=""([^""]*)""") {
                    $content = $content -replace "$([regex]::Escape($pattern))\s*class=""([^""]*)""", ('class="$1 ' + $replacement.Replace('class="', '').Replace('"', '') + '"')
                }
                # Altrimenti sostituisci style con class
                else {
                    $content = $content -replace [regex]::Escape($pattern), $replacement
                }
                
                $modified = $true
            }
        }
        
        if ($modified) {
            Set-Content $file $content -NoNewline
            Write-Host "[OK] $file - CSS inline rimosso" -ForegroundColor Green
            $count++
        }
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
