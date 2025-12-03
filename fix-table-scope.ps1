# Script migliorato per aggiungere scope="col" a TUTTI i th nelle thead
Write-Host "=== Fix Table Scope Attributes - Enhanced ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0
$thCount = 0

foreach ($file in $files) {
    $lines = Get-Content $file.FullName
    $modified = $false
    $inThead = $false
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        
        # Traccia se siamo dentro thead
        if ($line -match '<thead>') {
            $inThead = $true
        }
        
        if ($inThead) {
            # Pattern: <th qualsiasi_attributo> SENZA scope
            if ($line -match '<th(\s+[^>]*)?>' -and $line -notmatch 'scope=') {
                # Sostituisci <th con <th scope="col"
                $lines[$i] = $line -replace '<th(\s+)', '<th scope="col"$1'
                # Se non c'è spazio dopo th, aggiungi scope
                $lines[$i] = $lines[$i] -replace '<th>', '<th scope="col">'
                $modified = $true
                $thCount++
            }
        }
        
        if ($line -match '</thead>') {
            $inThead = $false
        }
    }
    
    if ($modified) {
        Set-Content $file.FullName $lines
        Write-Host "[OK] $($file.Name) - scope='col' aggiunto a tutti i th" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host "TH fixati: $thCount" -ForegroundColor Green
Write-Host ""
