# Script per aggiungere ARIA attributes a spinner
Write-Host "=== Fix ARIA Spinner Attributes ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern 1: <div id="divProcessing"> senza ARIA
    if ($content -match '<div id="divProcessing">') {
        # Verifica se non ha già ARIA attributes
        if ($content -notmatch 'id="divProcessing"[^>]*role="status"') {
            # Sostituisci con versione ARIA completa
            $content = $content -replace '<div id="divProcessing">', '<div id="divProcessing" class="d-none" role="status" aria-live="polite" aria-busy="true">'
            
            # Aggiungi sr-only text se non presente
            if ($content -notmatch '<p class="sr-only">') {
                $content = $content -replace '(<div id="divProcessing"[^>]*>\s*<br\s*/>\s*<br\s*/>\s*)', '$1<p class="sr-only">Caricamento in corso...</p>`r`n            '
            }
            
            # Marca testo visibile come aria-hidden
            $content = $content -replace '<p class="text-center">Processing data, please wait', '<p class="text-center" aria-hidden="true">Processing data, please wait'
            
            # Aggiungi role="presentation" alle immagini spinner se non presente
            $content = $content -replace '(<img[^>]*src="~/images/Spinner\.gif"[^>]*alt="[^"]*"(?![^>]*role=)[^>]*)(/>|>)', '$1 role="presentation"$2'
            
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - ARIA spinner attributes aggiunti" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
