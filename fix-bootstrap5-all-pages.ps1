# Script per convertire attributi Bootstrap 4 ? Bootstrap 5 in tutte le pagine
Write-Host "=== Bootstrap 5 Migration - Pages Fix ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml -Exclude "*.g.cs"
$totalFiles = 0
$totalReplacements = 0

$patterns = @{
    'data-toggle' = 'data-bs-toggle'
    'data-target' = 'data-bs-target'
    'data-dismiss' = 'data-bs-dismiss'
    'data-parent' = 'data-bs-parent'
    'data-slide-to' = 'data-bs-slide-to'
    'data-ride' = 'data-bs-ride'
}

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileChanged = $false
    $fileReplacements = 0
    
    foreach ($pattern in $patterns.Keys) {
        $replacement = $patterns[$pattern]
        
        if ($content -match $pattern) {
            $matches = [regex]::Matches($content, $pattern)
            $count = $matches.Count
            $content = $content -replace $pattern, $replacement
            $fileReplacements += $count
            $fileChanged = $true
        }
    }
    
    if ($fileChanged) {
        Set-Content $file.FullName $content -NoNewline
        $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
        Write-Host "[OK] $relativePath" -ForegroundColor Green
        Write-Host "     Replacements: $fileReplacements" -ForegroundColor Gray
        $totalFiles++
        $totalReplacements += $fileReplacements
    }
}

Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
Write-Host "Files modified: $totalFiles" -ForegroundColor Green
Write-Host "Total replacements: $totalReplacements" -ForegroundColor Green
Write-Host ""
Write-Host "=== Details ===" -ForegroundColor Yellow
foreach ($pattern in $patterns.Keys) {
    Write-Host "  $pattern ? $($patterns[$pattern])" -ForegroundColor Gray
}
Write-Host ""
