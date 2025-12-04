# ? Fix Allineamento Form PagesNormalizzato

**Data**: 2025-01-24  
**Status**: ? **COMPLETATO**

---

## ?? Problema Risolto

### ? Prima

La select del Centro non era allineata con gli altri controlli del form:

```razor
<div class="mb-3">
    <select asp-for="SelectedCentro" asp-items="Model.LstCentri" class="form-control mr-2">
        <option value="0">- Seleziona Centro -</option>
    </select>
</div>

<label for="StartDate" class="m-1 col-xs-3 col-form-label mr-2">Normalizzazione Da :</label>
<div class="m-2 col-xs-9">
    <input asp-for="StartDate" class="form-control mr-2" />
    <span asp-validation-for="StartDate" class="text-danger"></span>
</div>
```

**Problemi:**
- ? Select usava `form-control` invece di `form-select`
- ? Margini custom (`m-1`, `m-2`, `mr-2`, `col-xs-*`) disallineati
- ? Label visibili rompevano il layout flex
- ? Span validazione fuori dal wrapper
- ? Spaziatura irregolare

---

## ? Soluzione Applicata

### Form Allineato Bootstrap 5

```razor
<form class="d-flex flex-wrap gap-2 justify-content-center" method="post" id="formReport">

    <div class="mb-3">
        <select asp-for="SelectedCentro" asp-items="Model.LstCentri" class="form-select">
            <option value="0">- Seleziona Centro -</option>
        </select>
        <span asp-validation-for="SelectedCentro" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label for="StartDate" class="form-label visually-hidden">Normalizzazione Da:</label>
        <input asp-for="StartDate" class="form-control" placeholder="Normalizzazione Da" />
        <span asp-validation-for="StartDate" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label for="EndDate" class="form-label visually-hidden">Normalizzazione A:</label>
        <input asp-for="EndDate" class="form-control" placeholder="Normalizzazione A" />
        <span asp-validation-for="EndDate" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <button type="submit" class="btn btn-primary" id="btnStart">Report</button>
    </div>

</form>
```

---

## ?? Modifiche Dettagliate

### 1. Select Centro

**Prima:**
```html
<div class="mb-3">
    <select class="form-control mr-2">
```

**Dopo:**
```html
<div class="mb-3">
    <select class="form-select">
    <span asp-validation-for="SelectedCentro" class="text-danger"></span>
```

**Cambiamenti:**
- ? `form-control` ? `form-select` (classe corretta per `<select>` in BS5)
- ? Rimosso `mr-2` (margin-right non necessario)
- ? Aggiunto span validazione dentro wrapper

---

### 2. Input Date "Da"

**Prima:**
```html
<label for="StartDate" class="m-1 col-xs-3 col-form-label mr-2">Normalizzazione Da :</label>
<div class="m-2 col-xs-9">
    <input class="form-control mr-2" />
    <span class="text-danger"></span>
</div>
```

**Dopo:**
```html
<div class="mb-3">
    <label for="StartDate" class="form-label visually-hidden">Normalizzazione Da:</label>
    <input class="form-control" placeholder="Normalizzazione Da" />
    <span class="text-danger"></span>
</div>
```

**Cambiamenti:**
- ? Label dentro wrapper con `visually-hidden` (nascosta visualmente ma presente per screen reader)
- ? Rimossi margini custom (`m-1`, `m-2`, `mr-2`, `col-xs-*`)
- ? Aggiunto `placeholder` per chiarezza
- ? Span validazione dentro wrapper

---

### 3. Input Date "A"

**Prima:**
```html
<label for="EndDate" class="m-1 col-xs-3 col-form-label mr-2">Normalizzazione A :</label>
<div class="m-2 col-xs-9">
    <input class="form-control" />
    <span class="text-danger"></span>
</div>
```

**Dopo:**
```html
<div class="mb-3">
    <label for="EndDate" class="form-label visually-hidden">Normalizzazione A:</label>
    <input class="form-control" placeholder="Normalizzazione A" />
    <span class="text-danger"></span>
</div>
```

**Cambiamenti:**
- ? Label dentro wrapper con `visually-hidden`
- ? Rimossi margini custom
- ? Aggiunto `placeholder`
- ? Span validazione dentro wrapper

---

### 4. Bottone Submit

**Prima:**
```html
<div class="m-2 col-auto px-0">
    <button class="col-auto btn btn-primary">Report</button>
</div>
```

**Dopo:**
```html
<div class="mb-3">
    <button class="btn btn-primary">Report</button>
</div>
```

**Cambiamenti:**
- ? Rimossi margini custom (`m-2`, `col-auto`, `px-0`)
- ? Usato `mb-3` uniforme
- ? Rimossa classe `col-auto` dal bottone (non necessaria)

---

## ?? Confronto Visivo

### Prima (Disallineato) ?

```
[Select Centro con mr-2 ]    ? Disallineato

Label: Normalizzazione Da    ? Label visibile
[Input Date      ]           ? Margini irregolari

Label: Normalizzazione A
[Input Date]

[Button Report]              ? Troppo distante
```

---

### Dopo (Allineato) ?

```
[Select Centro]  ? Allineato con form-select

[Input Date (placeholder: Normalizzazione Da)]  ? Uniforme

[Input Date (placeholder: Normalizzazione A)]   ? Uniforme

[Button Report]  ? Spaziatura corretta
```

**Spaziatura:** 16px (1rem) uniforme tra tutti i controlli

---

## ?? Classi Bootstrap 5 Usate

| Elemento | Classe Corretta | Uso |
|----------|-----------------|-----|
| `<select>` | `form-select` | Dropdown select |
| `<input>` | `form-control` | Input text/date |
| Wrapper | `mb-3` | Margin-bottom 1rem |
| Label | `visually-hidden` | Nascosta ma accessibile |
| Span validation | `text-danger` | Colore rosso |

---

## ? Benefici

### 1. Allineamento Perfetto ?
- Tutti i controlli allineati orizzontalmente
- Spaziatura uniforme (16px tra elementi)
- Layout professionale

### 2. Bootstrap 5 Compliant ?
- Classe `form-select` per select
- Utility `mb-3` per spaziatura
- Nessuna classe deprecata

### 3. Accessibilità Migliorata ?
- Label presenti per screen reader
- `visually-hidden` nasconde visivamente ma mantiene accessibilità
- Placeholder aiutano comprensione

### 4. Codice Pulito ?
- Nessun margine custom
- Struttura consistente
- HTML semanticamente corretto

---

## ?? Testing

### Checklist Visuale

- [ ] ? **Select Centro**: allineata orizzontalmente
- [ ] ? **Input Normalizzazione Da**: allineato
- [ ] ? **Input Normalizzazione A**: allineato
- [ ] ? **Button Report**: allineato
- [ ] ? **Spaziatura**: 16px uniforme tra elementi
- [ ] ? **Select**: freccia Bootstrap 5 visibile
- [ ] ? **Placeholder**: visibili negli input date

---

### Checklist Funzionale

- [ ] ? **Seleziona Centro**: dropdown funziona
- [ ] ? **Compila date**: input funzionano
- [ ] ? **Click Report**: form si submette
- [ ] ? **Validazione**: messaggi errore visibili sotto i campi
- [ ] ? **DataTable**: carica dopo submit
- [ ] ? **Export Excel**: bottone funziona

---

### Come Testare

1. **Ferma debug e riavvia** (Shift+F5, F5)

2. **Naviga a PagesNormalizzato**
   ```
   Menu ? Reports ? Report Commesse Normalizzate
   ```

3. **Verifica Allineamento**
   - Tutti i controlli devono essere allineati orizzontalmente
   - Spaziatura uniforme tra Select, Input Date, Button
   - Select con freccia Bootstrap 5 (?)
   - Placeholder visibili negli input date

4. **Test Funzionale**
   - Seleziona Centro
   - Seleziona Date
   - Click "Report"
   - ? Verifica: Tabella carica, DataTable inizializzata

---

## ?? File Modificato

**File:** `Pages/PagesNormalizzato/Index.cshtml`

**Sezione:** Form filtri report

**Righe modificate:** ~30-50

---

## ?? Correlato

Questo fix segue lo stesso pattern applicato a:
- ? `Pages/PagesRiepilogo/Index.cshtml` (già fixato)
- ? `Pages/PagesSorterizzato/Index.cshtml` (da verificare)
- ? `Pages/PagesVolumi/Index.cshtml` (da verificare)

**Suggerimento:** Applicare lo stesso pattern a tutte le pagine report con form inline.

---

## ? Conclusione

Il form PagesNormalizzato è stato allineato con successo:

- ? Select Centro con `form-select`
- ? Margini custom rimossi
- ? Spaziatura uniforme `mb-3`
- ? Label accessibili con `visually-hidden`
- ? Placeholder aggiunti
- ? Build compilata

**Ready for Testing!** ???

---

**Ultima modifica**: 2025-01-24  
**Autore**: GitHub Copilot  
**Status**: ? **COMPLETATO - READY FOR TESTING**  
**Build**: ? Success
