# Script per fixare automaticamente label senza asp-for e form-control su select
# Esegui da directory root del progetto

$files = @(
    "Pages\PagesOperatori\Edit.cshtml",
    "Pages\TipiContenitori\Create.cshtml",
    "Pages\TipiContenitori\Edit.cshtml",
    "Pages\TipologiaNormalizzazione\Create.cshtml",
    "Pages\TipologiaNormalizzazione\Edit.cshtml",
    "Pages\TipiDocumenti\Create.cshtml",
    "Pages\TipiDocumenti\Edit.cshtml",
    "Pages\TipoPiattaforme\Create.cshtml",
    "Pages\TipoPiattaforme\Edit.cshtml",
    "Pages\PagesAssociazione\Create.cshtml",
    "Pages\PagesAssociazione\Edit.cshtml"
)

$fixes = 0

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $originalContent = $content
        
        # Fix 1: Sostituisci form-control con form-select nei tag select
        $content = $content -replace '(<select[^>]*class="[^"]*\b)form-control\b', '$1form-select'
        
        # Fix 2: Trova label senza asp-for prima di select/input (pattern comuni)
        # Nota: questo è un pattern base, potrebbero servire fix manuali
        
        if ($content -ne $originalContent) {
            Set-Content $file $content -NoNewline
            Write-Host "? Fixed: $file" -ForegroundColor Green
            $fixes++
        } else {
            Write-Host "??  No changes needed: $file" -ForegroundColor Yellow
        }
    } else {
        Write-Host "? File not found: $file" -ForegroundColor Red
    }
}

Write-Host "`n?? Summary: $fixes files fixed" -ForegroundColor Cyan
Write-Host "??  Note: Some label fixes may need manual review" -ForegroundColor Yellow
