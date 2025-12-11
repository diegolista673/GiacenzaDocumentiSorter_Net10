# Script per aggiornare i namespace delle pagine in PagesImpostazioni

$mappings = @(
    @{
        Folder = "Pages\PagesImpostazioni\PagesOperatori"
        OldNs = "GiacenzaSorterRm.Pages.PagesOperatori"
        NewNs = "GiacenzaSorterRm.Pages.PagesImpostazioni.PagesOperatori"
    },
    @{
        Folder = "Pages\PagesImpostazioni\PagesTipiDocumenti"
        OldNs = "GiacenzaSorterRm.Pages.TipiDocumenti"
        NewNs = "GiacenzaSorterRm.Pages.PagesImpostazioni.PagesTipiDocumenti"
    },
    @{
        Folder = "Pages\PagesImpostazioni\PagesTipologiaNormalizzazione"
        OldNs = "GiacenzaSorterRm.Pages.TipologiaNormalizzazione"
        NewNs = "GiacenzaSorterRm.Pages.PagesImpostazioni.PagesTipologiaNormalizzazione"
    },
    @{
        Folder = "Pages\PagesImpostazioni\PagesTipoPiattaforme"
        OldNs = "GiacenzaSorterRm.Pages.TipoPiattaforme"
        NewNs = "GiacenzaSorterRm.Pages.PagesImpostazioni.PagesTipoPiattaforme"
    }
)

foreach ($mapping in $mappings) {
    Write-Host "`nAggiornamento $($mapping.Folder)..." -ForegroundColor Yellow
    
    # Aggiorna file .cs
    $csFiles = Get-ChildItem "$($mapping.Folder)\*.cs"
    foreach ($file in $csFiles) {
        $content = Get-Content $file.FullName -Raw
        $newContent = $content -replace [regex]::Escape("namespace $($mapping.OldNs)"), "namespace $($mapping.NewNs)"
        if ($content -ne $newContent) {
            Set-Content $file.FullName $newContent -NoNewline
            Write-Host "  Updated: $($file.Name)" -ForegroundColor Green
        }
    }
    
    # Aggiorna file .cshtml
    $cshtmlFiles = Get-ChildItem "$($mapping.Folder)\*.cshtml"
    foreach ($file in $cshtmlFiles) {
        $content = Get-Content $file.FullName -Raw
        $newContent = $content -replace [regex]::Escape("@model $($mapping.OldNs)."), "@model $($mapping.NewNs)."
        if ($content -ne $newContent) {
            Set-Content $file.FullName $newContent -NoNewline
            Write-Host "  Updated: $($file.Name)" -ForegroundColor Green
        }
    }
}

Write-Host "`nCompletato!" -ForegroundColor Cyan
