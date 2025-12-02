# Script per sostituire IAppDbContext con GiacenzaSorterContext
Write-Host "=== Sostituzione IAppDbContext con GiacenzaSorterContext ===" -ForegroundColor Cyan
Write-Host ""

$files = @(
    "Pages\PagesAccettazione\Create.cshtml.cs",
    "Pages\PagesAssociazione\Create.cshtml.cs",
    "Pages\PagesAssociazione\Delete.cshtml.cs",
    "Pages\PagesAssociazione\Edit.cshtml.cs",
    "Pages\PagesAssociazione\Index.cshtml.cs",
    "Pages\PagesMacero\Index.cshtml.cs",
    "Pages\PagesNormalizzato\Index.cshtml.cs",
    "Pages\PagesNormalizzazione\Create.cshtml.cs",
    "Pages\PagesOperatori\Create.cshtml.cs",
    "Pages\PagesOperatori\Delete.cshtml.cs",
    "Pages\PagesOperatori\Edit.cshtml.cs",
    "Pages\PagesOperatori\Index.cshtml.cs",
    "Pages\PagesRicercaDispaccio\Index.cshtml.cs",
    "Pages\PagesRiepilogo\Edit.cshtml.cs",
    "Pages\PagesRiepilogo\Index.cshtml.cs",
    "Pages\PagesRiepilogoBancali\Edit.cshtml.cs",
    "Pages\PagesRiepilogoBancali\Index.cshtml.cs",
    "Pages\PagesSorter\Create.cshtml.cs",
    "Pages\PagesSorterizzato\Index.cshtml.cs",
    "Pages\PagesSpostaGiacenza\Create.cshtml.cs",
    "Pages\PagesVolumi\Index.cshtml.cs",
    "Pages\TipiContenitori\Create.cshtml.cs",
    "Pages\TipiContenitori\Delete.cshtml.cs",
    "Pages\TipiContenitori\Edit.cshtml.cs",
    "Pages\TipiContenitori\Index.cshtml.cs",
    "Pages\TipiDocumenti\Create.cshtml.cs",
    "Pages\TipiDocumenti\Delete.cshtml.cs",
    "Pages\TipiDocumenti\Edit.cshtml.cs",
    "Pages\TipiDocumenti\Index.cshtml.cs",
    "Pages\TipiLavorazioni\Create.cshtml.cs",
    "Pages\TipiLavorazioni\Delete.cshtml.cs",
    "Pages\TipiLavorazioni\Edit.cshtml.cs",
    "Pages\TipiLavorazioni\Index.cshtml.cs",
    "Pages\TipologiaNormalizzazione\Create.cshtml.cs",
    "Pages\TipologiaNormalizzazione\Delete.cshtml.cs",
    "Pages\TipologiaNormalizzazione\Edit.cshtml.cs",
    "Pages\TipologiaNormalizzazione\Index.cshtml.cs",
    "Pages\TipoPiattaforme\Create.cshtml.cs",
    "Pages\TipoPiattaforme\Delete.cshtml.cs",
    "Pages\TipoPiattaforme\Edit.cshtml.cs",
    "Pages\TipoPiattaforme\Index.cshtml.cs",
    "Pages\Index.cshtml.cs"
)

$replacements = @{
    "using GiacenzaSorterRm.Data;" = "using GiacenzaSorterRm.Models.Database;"
    "private readonly IAppDbContext _context;" = "private readonly GiacenzaSorterContext _context;"
    "IAppDbContext context" = "GiacenzaSorterContext context"
}

$count = 0

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $modified = $false
        
        foreach ($key in $replacements.Keys) {
            if ($content -match [regex]::Escape($key)) {
                $content = $content -replace [regex]::Escape($key), $replacements[$key]
                $modified = $true
            }
        }
        
        if ($modified) {
            Set-Content $file $content -NoNewline
            Write-Host "[OK] $file" -ForegroundColor Green
            $count++
        } else {
            Write-Host "[SKIP] $file - nessuna modifica necessaria" -ForegroundColor Gray
        }
    } else {
        Write-Host "[WARN] $file - file non trovato" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Completato ===" -ForegroundColor Cyan
Write-Host "File modificati: $count" -ForegroundColor Green
Write-Host ""
Write-Host "Prossimi passi:" -ForegroundColor Yellow
Write-Host "1. Elimina Data\IAppDbContext.cs" -ForegroundColor White
Write-Host "2. Verifica build: dotnet build" -ForegroundColor White
Write-Host ""
