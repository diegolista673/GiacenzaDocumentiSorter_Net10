# Script migliorato per aggiungere ARIA attributes alle modal
Write-Host "=== Fix ARIA Modal Attributes - Enhanced ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0
$detailed = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $modified = $false
    
    # Pattern: trova tutte le modal con ID
    $modalPattern = '<div\s+class="modal\s+fade"\s+id="([^"]+)"'
    $matches = [regex]::Matches($content, $modalPattern)
    
    foreach ($match in $matches) {
        $modalId = $match.Groups[1].Value
        $fullMatch = $match.Value
        
        # Verifica se non ha già role="dialog"
        if ($content -notmatch "id=`"$modalId`"[^>]{0,200}role=`"dialog`"") {
            
            # Aggiungi ARIA attributes
            $replacement = "<div class=`"modal fade`" id=`"$modalId`" tabindex=`"-1`" role=`"dialog`" aria-labelledby=`"${modalId}Title`" aria-modal=`"true`" aria-hidden=`"true`""
            $content = $content -replace [regex]::Escape($fullMatch), $replacement
            
            # Fix h5 modal-title per aggiungere ID corretto
            # Pattern: trova h5 nella modal corrente
            $titlePattern = "(<h5\s+class=`"modal-title`")(\s+id=`"[^`"]*`")?(`"[^>]*>)"
            $content = $content -replace $titlePattern, "`$1 id=`"${modalId}Title`"`$3"
            
            # Fix close button
            $content = $content -replace '(<button[^>]*class="close"[^>]*data-dismiss="modal")([^>]*)(>)', '$1 aria-label="Chiudi"$2$3'
            
            # Fix span × 
            $content = $content -replace '<span(\s+[^>]*)?>×</span>', '<span aria-hidden="true"$1>×</span>'
            
            $modified = $true
            $detailed += [PSCustomObject]@{
                File = $file.Name
                Path = $file.DirectoryName.Replace((Get-Location).Path, ".")
                ModalId = $modalId
            }
        }
    }
    
    if ($modified -and $content -ne $originalContent) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - ARIA modal attributes aggiunti" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Dettagli Modal Fixate ===" -ForegroundColor Yellow
$detailed | Format-Table -AutoSize

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host "Modal fixate: $($detailed.Count)" -ForegroundColor Green
Write-Host ""
