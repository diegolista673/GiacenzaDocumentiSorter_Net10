# Script per aggiungere ARIA attributes alle modal
Write-Host "=== Fix ARIA Modal Attributes ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern: <div class="modal fade" id="..." senza ARIA
    if ($content -match '<div class="modal fade"\s+id="([^"]+)"') {
        $modalId = $matches[1]
        
        # Verifica se non ha già role="dialog"
        if ($content -notmatch "id=`"$modalId`"[^>]*role=`"dialog`"") {
            # Aggiungi ARIA attributes alla modal
            $content = $content -replace "(<div class=`"modal fade`"\s+id=`"$modalId`")(`")", "`$1`" tabindex=`"-1`" role=`"dialog`" aria-labelledby=`"${modalId}Title`" aria-modal=`"true`" aria-hidden=`"true`$2"
            
            # Fix modal-title id per aria-labelledby
            $content = $content -replace "(<h5 class=`"modal-title`")(`")", "`$1 id=`"${modalId}Title`"`$2"
            
            # Aggiungi aria-label al close button se non presente
            $content = $content -replace '(<button[^>]*class="close"[^>]*data-dismiss="modal"(?![^>]*aria-label)[^>]*)(>)', '$1 aria-label="Chiudi"$2'
            
            # Aggiungi aria-hidden alla X
            $content = $content -replace '(<span[^>]*>×</span>)(?!\s*</button>)', '<span aria-hidden="true">×</span>'
            
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - ARIA modal attributes aggiunti" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
