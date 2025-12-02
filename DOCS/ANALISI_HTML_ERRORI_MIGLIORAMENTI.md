# ?? Report Analisi Codice HTML - GiacenzaSorterRm

**Data Analisi**: 2025-01-24  
**Progetto**: GiacenzaSorterRm (.NET 10)  
**Scope**: Analisi completa pagine Razor Pages  
**Status**: ?? Problemi Identificati

---

## ?? Indice

1. [Riepilogo Esecutivo](#riepilogo-esecutivo)
2. [Errori Critici](#errori-critici)
3. [Problemi di Accessibilità](#problemi-di-accessibilità)
4. [Problemi di Performance](#problemi-di-performance)
5. [Best Practices Non Seguite](#best-practices-non-seguite)
6. [Analisi per Pagina](#analisi-per-pagina)
7. [Raccomandazioni Prioritizzate](#raccomandazioni-prioritizzate)
8. [Piano di Azione](#piano-di-azione)

---

## ?? Riepilogo Esecutivo

### Statistiche Generali

| Metrica | Valore | Stato |
|---------|--------|-------|
| **Pagine Analizzate** | 60+ | ? |
| **Errori Critici** | 8 categorie | ?? |
| **Warning** | 12 categorie | ?? |
| **Best Practices** | 15 violazioni | ?? |
| **Accessibilità (WCAG)** | Non conforme | ? |
| **HTML5 Standard** | Parzialmente conforme | ?? |

### Problemi per Severità

- ?? **Critici**: 8 problemi (richiedono fix immediato)
- ?? **Warning**: 12 problemi (richiedono attenzione)
- ?? **Info**: 15 suggerimenti miglioramento

---

## ?? Errori Critici

### 1. CSS Inline Diffuso

**Severità**: ?? Alta  
**Pagine Affette**: Tutte (60+)  
**Descrizione**: Uso massiccio di attributi `style=""` inline

**Esempi:**
```html
<!-- ? MALE -->
<div class="card text-white bg-info mb-3" style="max-width: 100%;"></div>
<table style="width:100%" id="tableProduzione"></table>
<div style="display:none;" id="divProcessing"></div>
```

**Problemi:**
- ? Viola Content Security Policy (CSP)
- ? Difficile manutenzione
- ? Impossibile cache efficiente
- ? No riutilizzo stili
- ? Performance degradate

**Soluzione:**
```css
/* ? BENE - Creare classi CSS */
.card-full-width { max-width: 100%; }
.table-full-width { width: 100%; }
.hidden { display: none; }
```

**File da Creare:**
- `wwwroot/css/pages/macero.css`
- `wwwroot/css/pages/normalizzazione.css`
- `wwwroot/css/pages/sorter.css`
- `wwwroot/css/components/forms.css`
- `wwwroot/css/components/tables.css`

**Impatto:**
- ?? Sicurezza (CSP)
- ?? Performance
- ?? Manutenibilità

---

### 2. Attributi Evento Inline

**Severità**: ?? Alta  
**Pagine Affette**: ~30 pagine  
**Descrizione**: Uso di `onclick`, `onchange`, `oninput` inline

**Esempi:**
```html
<!-- ? MALE -->
<input oninput="this.value = this.value.toUpperCase()" />
<select onchange="change(this)" id="selectType"></select>
<button onclick="doSomething()">Click</button>
```

**Problemi:**
- ? Viola Content Security Policy (CSP)
- ? Non testabile
- ? Non separazione concerns
- ? Difficile debugging
- ? Rischio XSS

**Soluzione:**
```javascript
// ? BENE - Event listeners
document.getElementById('inputId').addEventListener('input', (e) => {
    e.target.value = e.target.value.toUpperCase();
});

// Funzione globale se necessaria per compatibilità
function change(selectElement) {
    // logica
}
window.change = change;
```

**Impatto:**
- ?? Sicurezza (CSP, XSS)
- ?? Manutenibilità
- ?? Testing

---

### 3. ID Duplicati in Loop

**Severità**: ?? Alta  
**Pagine Affette**: Partial con loop  
**Descrizione**: ID duplicati generati in `@foreach`

**Esempio:**
```razor
@* ? MALE - ID duplicati *@
@foreach (var item in Model.Items)
{
    <div id="itemDiv">@item.Name</div>
}
```

**Problemi:**
- ? HTML invalido (ID deve essere unico)
- ? JavaScript selectors broken
- ? Accessibilità compromessa
- ? DOM traversal fallisce

**Soluzione:**
```razor
@* ? BENE - ID univoci o usa classi *@
@foreach (var item in Model.Items)
{
    <div class="item-div" data-item-id="@item.Id">@item.Name</div>
}

@* O con ID univoco se necessario *@
@foreach (var item in Model.Items)
{
    <div id="itemDiv_@item.Id">@item.Name</div>
}
```

**File da Verificare:**
- `_RiepilogoAccettazioneBancali.cshtml`
- `_RiepilogoScatole.cshtml`
- `_RiepilogoBancali.cshtml`

**Impatto:**
- ?? HTML Validity
- ?? JavaScript Functionality
- ?? Accessibilità

---

### 4. Missing `for` Attribute in Labels

**Severità**: ?? Alta  
**Pagine Affette**: Tutte con form  
**Descrizione**: Label senza associazione esplicita all'input

**Esempio:**
```html
<!-- ? MALE -->
<label class="control-label">Data Normalizzazione</label>
<input asp-for="Scatole.DataNormalizzazione" class="form-control" />

<!-- ? MALE anche questo -->
<label for="Date">Dal :</label>
<input asp-for="StartDate" class="form-control" id="StartDate" />
<!-- "Date" non corrisponde a "StartDate" -->
```

**Problemi:**
- ? Accessibilità compromessa (screen reader)
- ? Click su label non focalizza input
- ? WCAG 2.1 non conforme

**Soluzione:**
```html
<!-- ? BENE -->
<label asp-for="Scatole.DataNormalizzazione" class="control-label"></label>
<input asp-for="Scatole.DataNormalizzazione" class="form-control" />

<!-- ? BENE - corrispondenza esplicita -->
<label for="StartDate">Dal :</label>
<input asp-for="StartDate" class="form-control" id="StartDate" />
```

**Impatto:**
- ?? Accessibilità (WCAG)
- ?? UX

---

### 5. Missing `alt` Attribute in Images

**Severità**: ?? Alta  
**Pagine Affette**: Pagine con spinner/immagini  
**Descrizione**: Immagini senza testo alternativo

**Esempio:**
```html
<!-- ? MALE -->
<img src="~/images/Spinner.gif" />
<img class="mx-auto d-block" src="~/images/Spinner.gif" />
```

**Problemi:**
- ? WCAG 2.1 non conforme (A)
- ? Screen reader non sa cosa mostrare
- ? SEO negativo

**Soluzione:**
```html
<!-- ? BENE -->
<img src="~/images/Spinner.gif" alt="Caricamento in corso..." />

<!-- ? Se decorativa -->
<img src="~/images/Spinner.gif" alt="" role="presentation" />
```

**File da Fix:**
- Tutte le pagine con `<img src="~/images/Spinner.gif">`

**Impatto:**
- ?? Accessibilità (WCAG Level A)
- ?? SEO
- ?? UX

---

### 6. Hidden Submit Buttons

**Severità**: ?? Media  
**Pagine Affette**: `_RiepilogoScatole.cshtml`, `_RiepilogoBancali.cshtml`  
**Descrizione**: Submit button nascosti con `hidden` attribute

**Esempio:**
```html
<!-- ?? QUESTIONABILE -->
@if ((User.IsInRole("ADMIN") || User.IsInRole("SUPERVISOR")))
{
    <input type="submit" id="btnElimina" hidden />
}
```

**Problemi:**
- ?? Pattern anti-intuitivo
- ?? Accessibilità keyboard navigation
- ?? Codice confuso (perché submit se hidden?)

**Soluzione:**
```javascript
// ? BENE - Trigger programmatico
document.getElementById('btnElimina').click(); // Non serve button hidden

// O meglio ancora
document.getElementById('formRiepilogo').dispatchEvent(new Event('submit'));
```

**Impatto:**
- ?? Codice Quality
- ?? Accessibilità

---

### 7. Inconsistent DataTable Button Configuration

**Severità**: ?? Media  
**Pagine Affette**: Tutte con DataTables  
**Descrizione**: Configurazione bottoni non uniforme

**Problemi Identificati:**

```javascript
// ? INCONSISTENTE - Alcune pagine usano
buttons: [{
    extend: 'excelHtml5', // ? Corretto
    // ...
}]

// ? Altre pagine usano
buttons: [{
    extend: 'excel', // ?? Deprecato in DataTables 2.x
    // ...
}]
```

**File con `extend: 'excel'`:**
- `PagesVolumi/Index.cshtml` (linea 156, 185)

**Soluzione:**
```javascript
// ? SEMPRE usare
buttons: [{
    extend: 'excelHtml5', // Standard DataTables 2.x
    autoFilter: true,
    text: '<i class="fas fa-file-excel"></i> Export to Excel',
    className: 'btn btn-primary'
}]
```

**Impatto:**
- ?? Compatibilità DataTables
- ?? Consistency

---

### 8. Form Without `novalidate`

**Severità**: ?? Bassa  
**Pagine Affette**: Tutte con validation custom  
**Descrizione**: Form senza `novalidate` per disabilitare HTML5 validation

**Esempio:**
```html
<!-- ?? POTENZIALE CONFLITTO -->
<form method="post" id="formReport">
    <input asp-for="StartDate" required class="form-control" />
    <!-- Validation HTML5 + Validation ASP.NET = Doppia validazione -->
</form>
```

**Problema:**
- Browser validation può interferire con validation server-side
- UX inconsistente

**Soluzione:**
```html
<!-- ? Se usi validation server-side -->
<form method="post" id="formReport" novalidate>
    <!-- ASP.NET validation only -->
</form>

<!-- ? O rimuovi `required` client-side -->
<form method="post" id="formReport">
    <input asp-for="StartDate" class="form-control" />
    <!-- Server-side validation in OnPost() -->
</form>
```

**Impatto:**
- ?? UX Consistency
- ?? Validation Control

---

## ?? Problemi di Accessibilità (WCAG 2.1)

### A. Level A (Critici)

#### A.1 Missing Alternative Text for Images
- **Rule**: [WCAG 1.1.1](https://www.w3.org/WAI/WCAG21/Understanding/non-text-content.html)
- **Status**: ? Non conforme
- **Fix**: Aggiungere `alt` a tutte le immagini

#### A.2 Label Association Missing
- **Rule**: [WCAG 1.3.1](https://www.w3.org/WAI/WCAG21/Understanding/info-and-relationships.html)
- **Status**: ? Non conforme
- **Fix**: Usare `asp-for` nei label o `for` attribute corretto

#### A.3 Keyboard Navigation
- **Rule**: [WCAG 2.1.1](https://www.w3.org/WAI/WCAG21/Understanding/keyboard.html)
- **Status**: ?? Parzialmente conforme
- **Fix**: Verificare tabindex, focus trap in modali

### B. Level AA (Importanti)

#### B.1 Color Contrast
- **Rule**: [WCAG 1.4.3](https://www.w3.org/WAI/WCAG21/Understanding/contrast-minimum.html)
- **Status**: ?? Non verificato
- **Fix**: Testare con tool (WebAIM Contrast Checker)

**Potenziali Problemi:**
```html
<!-- ?? Da verificare contrasto -->
<div class="card text-white bg-info">
    <!-- Testo bianco su sfondo blu info - contrasto OK? -->
</div>
```

#### B.2 Focus Visible
- **Rule**: [WCAG 2.4.7](https://www.w3.org/WAI/WCAG21/Understanding/focus-visible.html)
- **Status**: ?? Potenziale problema
- **Fix**: Non rimuovere `outline` da focus states

```css
/* ? MALE - Rimuove indicatore focus */
button:focus {
    outline: none;
}

/* ? BENE - Custom focus style */
button:focus {
    outline: 2px solid #0066cc;
    outline-offset: 2px;
}
```

### C. ARIA Attributes

#### C.1 Missing ARIA Labels
**Problemi:**
- Bottoni con solo icone senza label
- Modal senza `aria-labelledby`
- Loading spinner senza `aria-live`

**Esempio:**
```html
<!-- ? MALE -->
<button type="submit">
    <i class="fas fa-file-excel"></i>
</button>

<!-- ? BENE -->
<button type="submit" aria-label="Esporta in Excel">
    <i class="fas fa-file-excel"></i> Export to Excel
</button>
```

#### C.2 Missing `role` Attributes
```html
<!-- ?? Spinner dovrebbe avere role -->
<div id="divProcessing">
    <p>Processing data, please wait . . .</p>
    <img src="~/images/Spinner.gif" alt="Caricamento in corso..." role="status" aria-live="polite" />
</div>
```

---

## ?? Problemi di Performance

### P.1 CSS Inline Blocking Render

**Problema:**
```html
<!-- ? Blocca rendering -->
<style>
    #LastBancale[type='text'] {
        width: 100%;
        font-size: 48px;
        color: #0f08b2;
    }
</style>
```

**Impatto:**
- Render blocking
- No caching
- Ripetuto su ogni page load

**Soluzione:**
```html
<!-- ? CSS esterno -->
<link rel="stylesheet" href="~/css/pages/accettazione.css" asp-append-version="true" />
```

### P.2 jQuery Ready Duplicato

**Problema:**
```javascript
// ? Ripetuto in ogni pagina
$(document).ready(function () {
    $('#tableProduzione').DataTable({ scrollX: true });
});
```

**Soluzione:**
```javascript
// ? Usare pattern consistente
import { FetchHelpers } from '/js/fetch-helpers.js';

document.addEventListener('DOMContentLoaded', () => {
    FetchHelpers.initDataTable('tableProduzione', { scrollX: true });
});
```

### P.3 Script Non Minificati

**Problema:**
- Script inline non minificati
- No bundling
- No tree shaking

**Soluzione:**
- Usare bundling (WebPack/Vite)
- Minificazione automatica
- Code splitting

---

## ?? Best Practices Non Seguite

### BP.1 Semantic HTML

**Problema:**
```html
<!-- ? MALE - Generic div -->
<div class="card-header">
    <h5 class="card-title">Inserimento Bancale</h5>
</div>

<!-- ? BENE - Semantic header -->
<header class="card-header">
    <h2 class="card-title">Inserimento Bancale</h2>
</header>
```

### BP.2 Heading Hierarchy

**Problema:**
```html
<!-- ? MALE - Salta da h2 a h5 -->
<h2>Accettazione Bancale</h2>
<div class="card">
    <h5 class="card-title">Inserimento Bancale</h5>
</div>

<!-- ? BENE - Hierarchy corretta -->
<h1>Accettazione Bancale</h1>
<section class="card">
    <h2 class="card-title">Inserimento Bancale</h2>
</section>
```

### BP.3 Button vs Input Type Submit

**Status**: ? Già corretto nella maggior parte delle pagine migrate

```html
<!-- ? BENE -->
<button type="submit" class="btn btn-primary">Save</button>

<!-- ? MALE (legacy) -->
<input type="submit" value="Save" class="btn btn-primary" />
```

### BP.4 Form Field Grouping

**Problema:**
```html
<!-- ?? Manca fieldset per group -->
<form>
    <label>Dal:</label>
    <input type="date" />
    <label>Al:</label>
    <input type="date" />
</form>

<!-- ? BENE -->
<form>
    <fieldset>
        <legend>Periodo di riferimento</legend>
        <label for="startDate">Dal:</label>
        <input type="date" id="startDate" />
        <label for="endDate">Al:</label>
        <input type="date" id="endDate" />
    </fieldset>
</form>
```

### BP.5 Table Headers

**Problema:**
```html
<!-- ?? thead senza scope -->
<thead>
    <tr>
        <th>Scatola</th>
        <th>Data</th>
    </tr>
</thead>

<!-- ? BENE -->
<thead>
    <tr>
        <th scope="col">Scatola</th>
        <th scope="col">Data</th>
    </tr>
</thead>
```

### BP.6 Hidden Elements

**Problema:**
```html
<!-- ? MALE - style inline -->
<div id="divProcessing" style="display:none;">

<!-- ? BENE - classe CSS -->
<div id="divProcessing" class="d-none">
```

### BP.7 Placeholder vs Label

**Problema:**
```html
<!-- ? MALE - Solo placeholder -->
<input placeholder="Scatola" />

<!-- ? BENE - Label + placeholder -->
<label for="scatola">Codice Scatola</label>
<input id="scatola" placeholder="es. SC001" />
```

### BP.8 Loading States

**Problema:**
```html
<!-- ?? Spinner senza ARIA -->
<div id="divProcessing">
    <p>Processing data, please wait . . .</p>
    <img src="~/images/Spinner.gif" />
</div>

<!-- ? BENE -->
<div id="divProcessing" role="status" aria-live="polite" aria-busy="true">
    <p>Caricamento in corso...</p>
    <img src="~/images/Spinner.gif" alt="Caricamento" />
</div>
```

### BP.9 Modal Accessibility

**Problema:**
```html
<!-- ?? Modal senza ARIA -->
<div class="modal" id="confirm-submit">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5>Elimina Scatole</h5>
            </div>
        </div>
    </div>
</div>

<!-- ? BENE -->
<div class="modal" id="confirm-submit" 
     role="dialog" 
     aria-labelledby="modalTitle" 
     aria-modal="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modalTitle">Elimina Scatole</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Chiudi">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>
</div>
```

### BP.10 Empty Elements

**Problema:**
```html
<!-- ? MALE - div vuoto come placeholder -->
<div class="col-xs-6 center"></div>

<!-- ? BENE - rimuovi se inutile o usa semantically -->
<!-- Se necessario per layout, OK -->
```

---

## ?? Analisi per Pagina

### PagesMacero/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato a Fetch API** - OK
2. ? **CSS inline** - `style="max-width: 100%"`
3. ?? **Modale senza ARIA** - Modal `confirm-submit`
4. ?? **Spinner senza role** - `divProcessing`
5. ?? **img senza alt** - Spinner.gif
6. ? **Button configurazione** - Condizionale ADMIN OK

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt
- ?? Media: ARIA modal
- ?? Media: ARIA spinner

---

### PagesNormalizzato/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato a Fetch API** - OK
2. ? **CSS inline** - Multiple `style=`
3. ?? **img senza alt** - Spinner.gif
4. ?? **Spinner senza role**
5. ? **DataTable config** - OK con excelHtml5

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt
- ?? Media: ARIA spinner

---

### PagesSorter/Create.cshtml

**Problemi Identificati:**

1. ? **Form migrato a Fetch API** - OK
2. ? **CSS inline** - `style="max-width: 100%"`
3. ?? **oninput inline** - `oninput="this.value = this.value.toUpperCase()"`
4. ? **Button type submit** - OK

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: oninput ? addEventListener
- ?? Bassa: Semantic HTML

---

### PagesAccettazione/Create.cshtml

**Problemi Identificati:**

1. ? **2 Form migrati a Fetch API** - OK
2. ? **CSS inline** - Multiple
3. ?? **oninput inline** - toUpperCase
4. ?? **Label for mismatch** - `for="Date"` ma id diverso

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: Label for attribute
- ?? Alta: oninput ? addEventListener

---

### PagesAccettazione/_RiepilogoAccettazioneBancali.cshtml

**Problemi Identificati:**

1. ? **Form in loop migrato** - OK
2. ? **CSS inline nello `<style>`** - Custom styling
3. ?? **Potenziali ID duplicati** - Form in loop
4. ?? **img senza alt** - Input bancale

**Priorità Fix:**
- ?? Alta: `<style>` ? CSS file
- ?? Media: Verificare ID univoci
- ?? Alta: img alt

---

### PagesNormalizzazione/Create.cshtml

**Problemi Identificati:**

1. ? **Form + Vue.js + Fetch API** - OK
2. ? **CSS inline** - Multiple
3. ?? **oninput inline** - toUpperCase
4. ? **Vue.js integration** - OK
5. ? **Axios calls** - OK (mantenuti)

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: oninput ? Vue directive o addEventListener
- ?? Bassa: Vue best practices

---

### PagesRiepilogo/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato con logica condizionale** - OK
2. ? **CSS inline** - Multiple
3. ?? **Modal senza ARIA completo**
4. ?? **img senza alt**
5. ? **Checkbox select all** - OK
6. ? **Bottoni dinamici** - OK

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt
- ?? Media: ARIA completo modal

---

### PagesRiepilogo/_RiepilogoScatole.cshtml

**Problemi Identificati:**

1. ? **Form eliminazione migrato** - OK
2. ?? **Submit hidden** - Pattern questionabile
3. ?? **CSS inline** - `.css()` jQuery
4. ?? **Potenziali ID duplicati in loop**

**Priorità Fix:**
- ?? Media: Refactor submit hidden
- ?? Media: Verificare ID univoci
- ?? Bassa: jQuery .css() ? classe

---

### PagesRiepilogoBancali/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato** - OK
2. ? **CSS inline** - Multiple
3. ?? **onchange inline** - `onchange="change(this)"`
4. ?? **img senza alt**
5. ?? **Modal senza ARIA**

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: onchange ? addEventListener (già esposto window.change)
- ?? Alta: img alt
- ?? Media: ARIA modal

---

### PagesRiepilogoBancali/_RiepilogoBancali.cshtml

**Problemi Identificati:**

1. ? **Form eliminazione migrato** - OK
2. ?? **Submit hidden** - Pattern questionabile
3. ?? **CSS inline** - `.css()` jQuery
4. ?? **Potenziali ID duplicati in loop**

**Priorità Fix:**
- ?? Media: Refactor submit hidden
- ?? Media: Verificare ID univoci
- ?? Bassa: jQuery .css() ? classe

---

### PagesSorterizzato/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato** - OK
2. ? **CSS inline** - Multiple
3. ?? **img senza alt**
4. ? **DataTable config** - OK

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt

---

### PagesVolumi/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato** - OK
2. ? **CSS inline** - Multiple
3. ?? **img senza alt**
4. ? **DataTable `extend: 'excel'`** - Deprecato (deve essere `excelHtml5`)
5. ?? **Logica condizionale duplicata** - if/else per DataTable

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt
- ?? Alta: `extend: 'excel'` ? `excelHtml5`
- ?? Media: Refactor logica DataTable

---

### PagesRicercaDispaccio/Index.cshtml

**Problemi Identificati:**

1. ? **Form migrato** - OK
2. ? **CSS inline** - Multiple
3. ?? **img senza alt**
4. ?? **oninput inline** - toUpperCase

**Priorità Fix:**
- ?? Alta: CSS inline ? Classe
- ?? Alta: img alt
- ?? Alta: oninput ? addEventListener

---

### Shared/_Layout.cshtml

**Problemi da Verificare:**

1. ?? **Navbar accessibility**
2. ?? **Skip to content link** - Missing
3. ?? **Font Awesome icons senza aria-hidden**
4. ?? **Bootstrap modal global config**

**Priorità Fix:**
- ?? Media: Skip to content
- ?? Media: ARIA labels navbar
- ?? Bassa: Font Awesome aria-hidden

---

## ?? Raccomandazioni Prioritizzate

### Priorità 1 (Critiche - Fix Immediato)

#### 1. Rimuovere CSS Inline

**Azione:**
1. Creare file CSS organizzati per sezione
2. Estrarre tutti gli stili inline in classi
3. Usare utility classes Bootstrap quando possibile

**File da Creare:**
```
wwwroot/css/
??? pages/
?   ??? accettazione.css
?   ??? macero.css
?   ??? normalizzazione.css
?   ??? riepilogo.css
?   ??? sorter.css
?   ??? volumi.css
??? components/
?   ??? forms.css
?   ??? tables.css
?   ??? modals.css
?   ??? spinners.css
??? utilities/
    ??? custom-utilities.css
```

**Esempio Conversione:**
```css
/* wwwroot/css/components/forms.css */
.form-full-width { width: 100%; }
.input-uppercase { text-transform: uppercase; }

/* wwwroot/css/components/spinners.css */
.spinner-container {
    text-align: center;
    padding: 2rem 0;
}
.spinner-container img {
    display: block;
    margin: 0 auto;
}
```

**Tempo Stimato**: 8 ore  
**Impatto**: Alto (Sicurezza CSP, Performance)

---

#### 2. Aggiungere `alt` a Tutte le Immagini

**Azione:**
```html
<!-- Trovare tutte le occorrenze -->
<img src="~/images/Spinner.gif" />

<!-- Sostituire con -->
<img src="~/images/Spinner.gif" alt="Caricamento in corso..." />
```

**Script PowerShell:**
```powershell
$files = Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml
foreach($file in $files) {
    $content = Get-Content $file -Raw
    $content = $content -replace '<img\s+src="~/images/Spinner.gif"\s*/>', '<img src="~/images/Spinner.gif" alt="Caricamento in corso..." />'
    $content = $content -replace '<img\s+class="mx-auto d-block"\s+src="~/images/Spinner.gif"\s*/>', '<img class="mx-auto d-block" src="~/images/Spinner.gif" alt="Caricamento in corso..." />'
    Set-Content $file $content -NoNewline
}
```

**Tempo Stimato**: 30 minuti  
**Impatto**: Alto (WCAG Level A)

---

#### 3. Rimuovere Attributi Evento Inline

**Azione:**
Sostituire tutti `onclick`, `onchange`, `oninput` con event listeners

**Esempio:**
```html
<!-- PRIMA -->
<input oninput="this.value = this.value.toUpperCase()" />

<!-- DOPO -->
<input class="input-uppercase" data-transform="uppercase" />
```

```javascript
// Helper globale
document.querySelectorAll('[data-transform="uppercase"]').forEach(input => {
    input.addEventListener('input', (e) => {
        e.target.value = e.target.value.toUpperCase();
    });
});
```

**File da Modificare:**
- `PagesSorter/Create.cshtml`
- `PagesAccettazione/Create.cshtml`
- `PagesNormalizzazione/Create.cshtml`
- `PagesRicercaDispaccio/Index.cshtml`
- `PagesRiepilogoBancali/Index.cshtml`

**Tempo Stimato**: 4 ore  
**Impatto**: Alto (CSP, Sicurezza)

---

#### 4. Fix DataTables `extend: 'excel'` ? `excelHtml5`

**Azione:**
```javascript
// PRIMA
buttons: [{
    extend: 'excel', // ? Deprecato
}]

// DOPO
buttons: [{
    extend: 'excelHtml5', // ? Standard DataTables 2.x
}]
```

**File da Fix:**
- `PagesVolumi/Index.cshtml` (2 occorrenze)

**Tempo Stimato**: 15 minuti  
**Impatto**: Medio (Compatibilità)

---

### Priorità 2 (Importanti - Fix Breve Termine)

#### 5. Aggiungere ARIA Attributes a Modal

**Template:**
```html
<div class="modal fade" 
     id="confirm-submit" 
     tabindex="-1" 
     role="dialog" 
     aria-labelledby="modalTitle" 
     aria-modal="true"
     aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="modalTitle">Titolo Modal</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Chiudi">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <!-- Contenuto -->
            </div>
            <div class="modal-footer">
                <!-- Bottoni -->
            </div>
        </div>
    </div>
</div>
```

**File da Fix:**
- Tutte le pagine con modali (~10 file)

**Tempo Stimato**: 2 ore  
**Impatto**: Medio (Accessibilità WCAG AA)

---

#### 6. Aggiungere ARIA a Spinner

**Template:**
```html
<div id="divProcessing" 
     class="d-none" 
     role="status" 
     aria-live="polite" 
     aria-busy="true">
    <p class="sr-only">Caricamento in corso...</p>
    <p class="text-center" aria-hidden="true">Processing data, please wait...</p>
    <img class="mx-auto d-block" 
         src="~/images/Spinner.gif" 
         alt="Caricamento" 
         role="presentation" />
</div>
```

**File da Fix:**
- Tutte le pagine con spinner (~12 file)

**Tempo Stimato**: 1.5 ore  
**Impatto**: Medio (Accessibilità)

---

#### 7. Fix Label `for` Attributes

**Script Verifica:**
```powershell
# Trova label senza for o con for errato
Get-ChildItem -Path "Pages" -Recurse -Include *.cshtml | 
Select-String -Pattern '<label[^>]*>' | 
Where-Object { $_.Line -notmatch 'asp-for|for="[^"]+' }
```

**Azione:**
Usare sempre `asp-for` nei label o assicurare corrispondenza con ID input

**Tempo Stimato**: 3 ore  
**Impatto**: Medio (Accessibilità WCAG A)

---

#### 8. Refactor Hidden Submit Buttons

**File:**
- `_RiepilogoScatole.cshtml`
- `_RiepilogoBancali.cshtml`

**Azione:**
```javascript
// PRIMA
<input type="submit" id="btnElimina" hidden />
$('#submitModal').click(function () {
    $('#btnElimina').click();
});

// DOPO
$('#submitModal').click(function () {
    document.getElementById('formRiepilogo').dispatchEvent(new Event('submit'));
});
```

**Tempo Stimato**: 30 minuti  
**Impatto**: Basso (Code Quality)

---

### Priorità 3 (Miglioramenti - Medio Termine)

#### 9. Semantic HTML e Heading Hierarchy

**Azione:**
- Audit completo heading hierarchy
- Sostituire `<div>` con tag semantici (`<header>`, `<section>`, `<article>`)
- Fix heading jumps (h2 ? h5)

**Tempo Stimato**: 4 ore  
**Impatto**: Basso (SEO, Accessibilità)

---

#### 10. Table `scope` Attributes

**Azione:**
```html
<thead>
    <tr>
        <th scope="col">Scatola</th>
        <th scope="col">Data</th>
    </tr>
</thead>
```

**Tempo Stimato**: 2 ore  
**Impatto**: Basso (Accessibilità)

---

#### 11. Color Contrast Audit

**Tool:**
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Chrome DevTools Lighthouse](https://developers.google.com/web/tools/lighthouse)

**Azione:**
1. Testare tutti i colori usati
2. Fix contrasti < 4.5:1 (WCAG AA)
3. Documentare palette approvata

**Tempo Stimato**: 3 ore  
**Impatto**: Medio (WCAG AA)

---

#### 12. Skip to Content Link

**Azione:**
```html
<!-- In _Layout.cshtml, prima del navbar -->
<a href="#main-content" class="skip-to-content">
    Salta al contenuto principale
</a>

<!-- CSS -->
.skip-to-content {
    position: absolute;
    top: -40px;
    left: 0;
    background: #000;
    color: #fff;
    padding: 8px;
    text-decoration: none;
    z-index: 9999;
}

.skip-to-content:focus {
    top: 0;
}

<!-- Nel main -->
<main id="main-content" tabindex="-1">
    <!-- Contenuto pagine -->
</main>
```

**Tempo Stimato**: 30 minuti  
**Impatto**: Basso (Accessibilità)

---

#### 13. Font Awesome Icons Accessibility

**Azione:**
```html
<!-- PRIMA -->
<i class="fas fa-file-excel"></i>

<!-- DOPO - Icon decorativa -->
<i class="fas fa-file-excel" aria-hidden="true"></i> Export to Excel

<!-- DOPO - Icon con significato -->
<i class="fas fa-exclamation-circle" aria-label="Attenzione" role="img"></i>
```

**Tempo Stimato**: 2 ore  
**Impatto**: Basso (Accessibilità)

---

#### 14. Form `novalidate` Strategy

**Decisione:**
Scegliere strategia validation consistente:

**Opzione A**: Client + Server
```html
<form method="post"> <!-- HTML5 validation -->
    <input required asp-for="Field" />
</form>
```

**Opzione B**: Solo Server (Raccomandato)
```html
<form method="post" novalidate>
    <input asp-for="Field" /> <!-- Server validation only -->
    <span asp-validation-for="Field"></span>
</form>
```

**Tempo Stimato**: 1 ora (decisione + update)  
**Impatto**: Basso (UX Consistency)

---

#### 15. Placeholder vs Label Audit

**Azione:**
Assicurare che ogni input con placeholder abbia anche label visibile

```html
<!-- ? MALE -->
<input placeholder="Scatola" />

<!-- ? BENE -->
<label for="scatola">Codice Scatola</label>
<input id="scatola" placeholder="es. SC001" />
```

**Tempo Stimato**: 2 ore  
**Impatto**: Basso (Accessibilità)

---

## ?? Piano di Azione

### Sprint 1 (1 settimana) - Priorità Critiche

**Obiettivo**: Fix problemi critici accessibilità e sicurezza

| Task | Tempo | Responsabile | Status |
|------|-------|--------------|--------|
| CSS Inline ? File esterni | 8h | Dev | ? |
| img `alt` attributes | 0.5h | Dev | ? |
| Event listeners (no inline) | 4h | Dev | ? |
| DataTables `excelHtml5` | 0.25h | Dev | ? |
| **TOTALE** | **12.75h** | | |

**Deliverable**: Build con 0 problemi critici

---

### Sprint 2 (1 settimana) - Accessibilità

**Obiettivo**: WCAG 2.1 Level A compliance

| Task | Tempo | Responsabile | Status |
|------|-------|--------------|--------|
| ARIA Modal attributes | 2h | Dev | ? |
| ARIA Spinner attributes | 1.5h | Dev | ? |
| Label `for` fix | 3h | Dev | ? |
| Hidden submit refactor | 0.5h | Dev | ? |
| **TOTALE** | **7h** | | |

**Deliverable**: WCAG Level A compliant

---

### Sprint 3 (1 settimana) - Best Practices

**Obiettivo**: Codice pulito e manutenibile

| Task | Tempo | Responsabile | Status |
|------|-------|--------------|--------|
| Semantic HTML audit | 4h | Dev | ? |
| Table scope attributes | 2h | Dev | ? |
| Color contrast audit | 3h | Dev | ? |
| Skip to content | 0.5h | Dev | ? |
| Font Awesome ARIA | 2h | Dev | ? |
| **TOTALE** | **11.5h** | | |

**Deliverable**: WCAG Level AA target

---

### Sprint 4 (3 giorni) - Validation & Testing

**Obiettivo**: Verifica e testing completo

| Task | Tempo | Responsabile | Status |
|------|-------|--------------|--------|
| HTML Validator (W3C) | 2h | QA | ? |
| Lighthouse Audit | 2h | QA | ? |
| Screen Reader Testing | 3h | QA | ? |
| Browser Testing | 2h | QA | ? |
| Regression Testing | 3h | QA | ? |
| **TOTALE** | **12h** | | |

**Deliverable**: Report conformità completo

---

## ?? Testing Checklist

### HTML Validation

- [ ] [W3C Markup Validator](https://validator.w3.org/)
- [ ] [Nu HTML Checker](https://validator.w3.org/nu/)
- [ ] No errori HTML5
- [ ] No warning critici

### Accessibilità

- [ ] [WAVE Web Accessibility Evaluation Tool](https://wave.webaim.org/)
- [ ] [axe DevTools](https://www.deque.com/axe/devtools/)
- [ ] [Lighthouse Accessibility Audit](https://developers.google.com/web/tools/lighthouse)
- [ ] Screen Reader Testing (NVDA/JAWS)
- [ ] Keyboard Navigation Testing
- [ ] Color Contrast Verification

### Performance

- [ ] [Lighthouse Performance](https://developers.google.com/web/tools/lighthouse)
- [ ] [WebPageTest](https://www.webpagetest.org/)
- [ ] CSS Render Blocking
- [ ] JavaScript Load Time
- [ ] Image Optimization

### SEO

- [ ] Lighthouse SEO Audit
- [ ] Meta tags validation
- [ ] Heading hierarchy
- [ ] Semantic HTML

### Browser Compatibility

- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Edge (latest)
- [ ] Safari (latest, se richiesto)

---

## ?? Metriche di Successo

### Before

| Metrica | Valore |
|---------|--------|
| **HTML Errors** | ~50+ |
| **WCAG Level A** | ? Non conforme |
| **WCAG Level AA** | ? Non conforme |
| **Lighthouse Accessibility** | ~60-70 |
| **CSS Inline** | ~100+ occorrenze |
| **Event Inline** | ~20+ occorrenze |

### Target After

| Metrica | Target |
|---------|--------|
| **HTML Errors** | 0 |
| **WCAG Level A** | ? Conforme |
| **WCAG Level AA** | ? Conforme (target) |
| **Lighthouse Accessibility** | 90+ |
| **CSS Inline** | 0 |
| **Event Inline** | 0 |

---

## ?? Riferimenti

### Standard e Guidelines

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [HTML5 Specification](https://html.spec.whatwg.org/)
- [MDN Web Docs - HTML](https://developer.mozilla.org/en-US/docs/Web/HTML)
- [MDN Web Docs - Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)

### Tools

- [W3C Markup Validator](https://validator.w3.org/)
- [WAVE Accessibility Tool](https://wave.webaim.org/)
- [axe DevTools](https://www.deque.com/axe/devtools/)
- [Chrome DevTools Lighthouse](https://developers.google.com/web/tools/lighthouse)
- [WebAIM Contrast Checker](https://webaim.org/resources/contrastchecker/)

### Bootstrap Accessibility

- [Bootstrap 5 Accessibility](https://getbootstrap.com/docs/5.0/getting-started/accessibility/)
- [Bootstrap Modal Accessibility](https://getbootstrap.com/docs/5.0/components/modal/#accessibility)

### DataTables

- [DataTables Accessibility](https://datatables.net/extensions/responsive/examples/initialisation/accessibility.html)
- [DataTables Buttons Documentation](https://datatables.net/extensions/buttons/)

---

## ?? Note Finali

### Priorità Business

Se il tempo è limitato, concentrarsi su:

1. **Sicurezza (CSP)** - CSS/Event inline ? File esterni
2. **WCAG Level A** - Accessibilità base (alt, label, keyboard)
3. **DataTables Compatibility** - Fix deprecati

### Maintenance

Dopo il fix iniziale, implementare:

- **Linting**: HTML/CSS linting nel pipeline CI/CD
- **Pre-commit hooks**: Validazione automatica
- **Code reviews**: Checklist accessibilità
- **Monitoring**: Lighthouse CI per regression

### Team Education

- Workshop WCAG 2.1 basics
- Training accessibilità keyboard
- Best practices HTML5/CSS3
- DataTables 2.x migration guide

---

**Ultima modifica**: 2025-01-24  
**Autore**: GitHub Copilot  
**Status**: ?? Report Completo  
**Prossimo Step**: Prioritizzare fix e iniziare Sprint 1
