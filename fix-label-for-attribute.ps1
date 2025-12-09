# Script per aggiungere attributo 'for' alle label e 'id' ai controlli
# Fix SonarQube: "A form label must be associated with a control"

$files = @(
    'Pages\PagesNormalizzazione\Create.cshtml',
    'Pages\PagesRiepilogo\Edit.cshtml',
    'Pages\PagesRiepilogoBancali\Edit.cshtml',
    'Pages\PagesSorter\Create.cshtml',
    'Pages\PagesSpostaGiacenza\Create.cshtml',
    'Pages\TipiContenitori\Create.cshtml',
    'Pages\TipiContenitori\Edit.cshtml',
    'Pages\TipiDocumenti\Create.cshtml',
    'Pages\TipiDocumenti\Edit.cshtml',
    'Pages\TipiLavorazioni\Create.cshtml',
    'Pages\TipiLavorazioni\Edit.cshtml',
    'Pages\TipologiaNormalizzazione\Create.cshtml',
    'Pages\TipologiaNormalizzazione\Edit.cshtml',
    'Pages\TipoPiattaforme\Create.cshtml',
    'Pages\TipoPiattaforme\Edit.cshtml'
)

$totalFixed = 0
$summary = @()

foreach ($file in $files) {
    if (!(Test-Path $file)) {
        Write-Host "??  File not found: $file" -ForegroundColor Yellow
        continue
    }

    $content = Get-Content $file -Raw
    $originalContent = $content
    $fileFixed = 0

    # Pattern 1: <label asp-for="XXX" class="control-label"> ? aggiungi for="XXX"
    # Estrae il valore di asp-for e crea l'attributo for con underscore
    $content = $content -replace '<label\s+asp-for="([^"]+)"\s+class="control-label">', {
        param($match)
        $aspFor = $match.Groups[1].Value
        $forId = $aspFor -replace '\.', '_'
        $fileFixed++
        "<label for=`"$forId`" asp-for=`"$aspFor`" class=`"form-label`">"
    }

    # Pattern 2: <label class="control-label" asp-for="XXX"> ? aggiungi for="XXX"
    $content = $content -replace '<label\s+class="control-label"\s+asp-for="([^"]+)">', {
        param($match)
        $aspFor = $match.Groups[1].Value
        $forId = $aspFor -replace '\.', '_'
        $fileFixed++
        "<label for=`"$forId`" class=`"form-label`" asp-for=`"$aspFor`">"
    }

    # Pattern 3: <input asp-for="XXX" ? aggiungi id="XXX" se manca
    $content = $content -replace '<input\s+asp-for="([^"]+)"(?!\s+[^>]*id=)([^>]*)>', {
        param($match)
        $aspFor = $match.Groups[1].Value
        $rest = $match.Groups[2].Value
        $inputId = $aspFor -replace '\.', '_'
        "<input asp-for=`"$aspFor`" id=`"$inputId`"$rest>"
    }

    # Pattern 4: <select asp-for="XXX" ? aggiungi id="XXX" se manca
    $content = $content -replace '<select\s+asp-for="([^"]+)"(?!\s+[^>]*id=)([^>]*)>', {
        param($match)
        $aspFor = $match.Groups[1].Value
        $rest = $match.Groups[2].Value
        $selectId = $aspFor -replace '\.', '_'
        "<select asp-for=`"$aspFor`" id=`"$selectId`"$rest>"
    }

    # Pattern 5: <textarea asp-for="XXX" ? aggiungi id="XXX" se manca
    $content = $content -replace '<textarea\s+asp-for="([^"]+)"(?!\s+[^>]*id=)([^>]*)>', {
        param($match)
        $aspFor = $match.Groups[1].Value
        $rest = $match.Groups[2].Value
        $textareaId = $aspFor -replace '\.', '_'
        "<textarea asp-for=`"$aspFor`" id=`"$textareaId`"$rest>"
    }

    if ($content -ne $originalContent) {
        Set-Content -Path $file -Value $content -NoNewline -Encoding UTF8
        Write-Host "? Fixed: $file ($fileFixed changes)" -ForegroundColor Green
        $totalFixed++
        $summary += [PSCustomObject]@{
            File = $file
            Changes = $fileFixed
        }
    } else {
        Write-Host "??  No changes needed: $file" -ForegroundColor Cyan
    }
}

Write-Host "`n?? Summary:" -ForegroundColor Magenta
Write-Host "????????????????????????????????????" -ForegroundColor Magenta
Write-Host "Total files fixed: $totalFixed / $($files.Count)" -ForegroundColor Green
if ($summary.Count -gt 0) {
    Write-Host "`nDetailed changes:" -ForegroundColor Yellow
    $summary | Format-Table -AutoSize
}
Write-Host "`n? Script completed!" -ForegroundColor Green
