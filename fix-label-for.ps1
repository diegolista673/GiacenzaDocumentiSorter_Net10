# Script per fixare label for attributes
Write-Host "=== Fix Label For Attributes ===" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
$count = 0
$issues = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Pattern 1: <label for="Date">...Da/Dal... seguita da input asp-for="StartDate"
    if ($content -match '<label\s+for="Date"[^>]*>.*?D[ai][l:]?\s*[:]?</label>\s*\r?\n\s*<div[^>]*>\s*\r?\n\s*<input\s+asp-for="StartDate"') {
        $content = $content -replace '<label\s+for="Date"([^>]*)>([^<]*D[ai][l:]?\s*[:]?)</label>(\s*\r?\n\s*<div[^>]*>\s*\r?\n\s*<input\s+asp-for="StartDate")', '<label for="StartDate"$1>$2</label>$3'
        $modified = $true
        $issues += "Fixed: for='Date' -> for='StartDate' in $($file.Name)"
    }
    
    # Pattern 2: <label for="Date">...A/Al... seguita da input asp-for="EndDate"
    if ($content -match '<label\s+for="Date"[^>]*>.*?A[l:]?\s*[:]?</label>\s*\r?\n\s*<div[^>]*>\s*\r?\n\s*<input\s+asp-for="EndDate"') {
        $content = $content -replace '<label\s+for="Date"([^>]*)>([^<]*A[l:]?\s*[:]?)</label>(\s*\r?\n\s*<div[^>]*>\s*\r?\n\s*<input\s+asp-for="EndDate")', '<label for="EndDate"$1>$2</label>$3'
        $modified = $true
        $issues += "Fixed: for='Date' -> for='EndDate' in $($file.Name)"
    }
    
    # Pattern 3: Fix syntax error in img tag: / role= -> role=
    if ($content -match '(<img[^>]*)\s+/\s+role="') {
        $content = $content -replace '(<img[^>]*)\s+/\s+role="', '$1 role="'
        $modified = $true
        $issues += "Fixed: img syntax error (/ role=) in $($file.Name)"
    }
    
    # Pattern 4: Fix double class attributes
    if ($content -match '<div[^>]*class="[^"]*"[^>]*class="[^"]*"') {
        $content = $content -replace '<div\s+class="([^"]*)"([^>]*)class="([^"]*)"', '<div class="$1 $3"$2'
        $modified = $true
        $issues += "Fixed: double class attributes in $($file.Name)"
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "[OK] $($file.Name) - Label for attributes fixati" -ForegroundColor Green
        $count++
    }
}

Write-Host ""
if ($issues.Count -gt 0) {
    Write-Host "=== Issues Fixed ===" -ForegroundColor Yellow
    $issues | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
