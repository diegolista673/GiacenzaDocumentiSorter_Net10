# ?? Guida Migrazione jQuery Ajax ? Fetch API

**Progetto**: GiacenzaSorterRm  
**Target**: Rimozione dipendenza `jquery.unobtrusive-ajax.js`  
**Priorità**: ?? ALTA  
**Tempo stimato**: 2-3 giorni  
**Data**: 2025

---

## ?? Indice

1. [Panoramica](#panoramica)
2. [File Helper Fetch API](#file-helper-fetch-api)
3. [File da Migrare](#file-da-migrare)
4. [Esempi di Migrazione Dettagliati](#esempi-di-migrazione-dettagliati)
5. [Pattern Comuni](#pattern-comuni)
6. [Checklist Implementazione](#checklist-implementazione)
7. [Testing](#testing)

---

## Panoramica

### ?? Statistiche Progetto

**File con jQuery Unobtrusive Ajax identificati:**
- ? `Pages/PagesNormalizzazione/Create.cshtml` (1 form + chiamate Axios)
- ? `Pages/PagesAccettazione/Create.cshtml` (2 form)
- ? `Pages/PagesSorter/Create.cshtml` (1 form)
- ? `Pages/PagesSpostaGiacenza/Create.cshtml` (1 form)
- ? `Pages/PagesNormalizzato/Index.cshtml` (1 form)
- ? `Pages/PagesMacero/Index.cshtml` (1 form)
- ? `Pages/TipologiaNormalizzazione/Index.cshtml` (DataTables solo)

**Totale form da migrare: 7**

### ? Problemi Attuali

| Problema | Impatto | Criticità |
|----------|---------|-----------|
| Dipendenza libreria obsoleta | Sicurezza/Manutenzione | ?? Alta |
| Performance subottimale | UX | ?? Media |
| Debugging difficile | Sviluppo | ?? Media |
| Syntax non standard | Manutenibilità | ?? Bassa |
| Bundle size inutile | Performance | ?? Bassa |

### ? Benefici Migrazione

| Aspetto | Prima (jQuery Ajax) | Dopo (Fetch API) | Miglioramento |
|---------|---------------------|------------------|---------------|
| **Bundle Size** | ~85KB (jQuery) | 0KB (nativo) | -85KB |
| **Performance** | Overhead jQuery | Nativo browser | +30-40% |
| **Manutenibilità** | Libreria esterna | Standard web | ? |
| **Async/Await** | Callback hell | Nativo ES6+ | ? |
| **TypeScript** | Limitato | Completo | ? |

---

## File Helper Fetch API

### Step 1: Creare `wwwroot/js/fetch-helpers.js`

```javascript
/**
 * Fetch API Helpers per GiacenzaSorterRm
 * Sostituisce jquery.unobtrusive-ajax.js
 * 
 * @version 1.0
 * @author GiacenzaSorterRm Team
 */

export const FetchHelpers = {
    
    /**
     * Esegue una POST request con FormData e antiforgery token
     * Equivalente a data-ajax="true" con data-ajax-method="post"
     * 
     * @param {string} url - URL handler Razor Pages (es: "?handler=InsertScatola")
     * @param {FormData} formData - Dati form da inviare
     * @param {string} updateElementId - ID elemento da aggiornare con risposta HTML
     * @param {Function} onSuccess - Callback opzionale dopo successo
     * @param {Function} onError - Callback opzionale in caso di errore
     * @returns {Promise<string>} HTML response
     */
    async postFormWithUpdate(url, formData, updateElementId, onSuccess = null, onError = null) {
        try {
            // Ottieni antiforgery token
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            // Headers (FormData gestisce Content-Type automaticamente)
            const headers = {
                'RequestVerificationToken': token
            };

            // Fetch request
            const response = await fetch(url, {
                method: 'POST',
                headers: headers,
                body: formData,
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            // Ottieni HTML response
            const html = await response.text();
            
            // Aggiorna elemento DOM
            const targetElement = document.getElementById(updateElementId);
            if (targetElement) {
                targetElement.innerHTML = html;
            } else {
                console.warn(`Elemento #${updateElementId} non trovato nel DOM`);
            }

            // Callback successo
            if (onSuccess) {
                onSuccess(html, response);
            }

            return html;

        } catch (error) {
            console.error('Fetch POST error:', error);
            
            // Callback errore
            if (onError) {
                onError(error);
            } else {
                // Gestione errore di default
                alert('Si è verificato un errore. Riprova.');
            }
            
            throw error;
        }
    },

    /**
     * Esegue una GET request con query parameters
     * 
     * @param {string} url - URL endpoint
     * @param {Object} params - Query parameters (es: {idCommessa: 1, idTipologia: 2})
     * @param {Function} onSuccess - Callback successo
     * @param {Function} onError - Callback errore
     * @returns {Promise<any>} Parsed JSON response
     */
    async get(url, params = {}, onSuccess = null, onError = null) {
        try {
            // Costruisci URL con query params
            const queryString = new URLSearchParams(params).toString();
            const fullUrl = queryString ? `${url}${url.includes('?') ? '&' : '?'}${queryString}` : url;

            // Aggiungi antiforgery token agli headers (best practice)
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            const headers = token ? { 'RequestVerificationToken': token } : {};

            const response = await fetch(fullUrl, {
                method: 'GET',
                headers: headers,
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            // Parse JSON (Axios compatibility)
            const contentType = response.headers.get('content-type');
            let data;
            
            if (contentType && contentType.includes('application/json')) {
                data = await response.json();
            } else {
                data = await response.text();
            }

            if (onSuccess) {
                onSuccess(data, response);
            }

            return data;

        } catch (error) {
            console.error('Fetch GET error:', error);
            
            if (onError) {
                onError(error);
            }
            
            throw error;
        }
    },

    /**
     * Mostra spinner/loading indicator
     * @param {string} elementId - ID elemento spinner
     */
    showSpinner(elementId) {
        const spinner = document.getElementById(elementId);
        if (spinner) {
            spinner.style.display = 'block';
        }
    },

    /**
     * Nascondi spinner/loading indicator
     * @param {string} elementId - ID elemento spinner
     */
    hideSpinner(elementId) {
        const spinner = document.getElementById(elementId);
        if (spinner) {
            spinner.style.display = 'none';
        }
    },

    /**
     * Reinizializza DataTables dopo aggiornamento DOM
     * @param {string} tableId - ID tabella DataTables
     * @param {Object} options - Opzioni DataTables
     */
    reinitDataTable(tableId, options = {}) {
        const defaultOptions = {
            scrollX: true,
            order: [],
            ...options
        };

        // Distruggi istanza esistente
        const existingTable = $.fn.DataTable.isDataTable(`#${tableId}`);
        if (existingTable) {
            $(`#${tableId}`).DataTable().destroy();
        }

        // Ricrea DataTable
        $(`#${tableId}`).DataTable(defaultOptions);
    },

    /**
     * Vai all'ultima pagina di DataTable e fai il refresh
     * @param {string} tableId - ID tabella DataTables
     */
    dataTableGoToLastPage(tableId) {
        const table = $(`#${tableId}`).DataTable();
        table.page('last').draw('page');
    },

    /**
     * Pulisci valori di input form
     * @param {string[]} inputIds - Array di ID input da pulire
     */
    clearInputs(inputIds) {
        inputIds.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.value = '';
            }
        });
    }
};

// Export default per compatibilità
export default FetchHelpers;
```

---

## File da Migrare

### ?? Priorità 1 - Form con Submit Frequente

#### 1. `Pages/PagesNormalizzazione/Create.cshtml`
**Complessità**: ?? Alta (Vue.js + Axios + jQuery Ajax)  
**Form Ajax**: 1 (InsertScatola)  
**Chiamate Axios da migrare**: 3 (AssociazioneTipologia, AssociazioneContenitore, ScatolaMondo)

#### 2. `Pages/PagesAccettazione/Create.cshtml`
**Complessità**: ?? Media  
**Form Ajax**: 2 (InsertDispacci, CreaNuovoBancale)  
**DataTables**: 2 (tableProduzione, tableProduzioneDispacci)

#### 3. `Pages/PagesSorter/Create.cshtml`
**Complessità**: ?? Bassa  
**Form Ajax**: 1 (InsertScatola)  
**DataTables**: 1 (tableProduzione)

#### 4. `Pages/PagesSpostaGiacenza/Create.cshtml`
**Complessità**: ?? Bassa  
**Form Ajax**: 1 (AggiornaScatola)  
**DataTables**: 1 (tableProduzione)

### ?? Priorità 2 - Form con Report

#### 5. `Pages/PagesNormalizzato/Index.cshtml`
**Complessità**: ?? Bassa  
**Form Ajax**: 1 (Report)  
**Spinner**: ? Presente

#### 6. `Pages/PagesMacero/Index.cshtml`
**Complessità**: ?? Bassa  
**Form Ajax**: 1 (ReportNormalizzazione)  
**Spinner**: ? Presente

### ?? Priorità 3 - Solo DataTables

#### 7. `Pages/TipologiaNormalizzazione/Index.cshtml`
**Complessità**: ?? Bassa  
**Form Ajax**: ? Nessuno  
**Azione**: Rimuovere solo include `jquery.unobtrusive-ajax.js`

---

## Esempi di Migrazione Dettagliati

### Esempio 1: PagesSorter/Create.cshtml (Caso Semplice)

#### ? PRIMA (jQuery Unobtrusive Ajax)

```razor
<form method="post"
      data-ajax="true"
      data-ajax-method="post"
      data-ajax-update="#resultDiv"
      data-ajax-mode="replace"
      data-ajax-url="?handler=InsertScatola"
      data-ajax-success="OnSuccessRequest">
    
    <input asp-for="Scatole.DataSorter" class="form-control" />
    <textarea asp-for="Scatole.Note" class="form-control"></textarea>
    <input asp-for="Scatole.Scatola" class="form-control" />
    
    <input type="submit" value="Save" class="btn btn-primary" />
</form>

@section scripts {
    <script src="~/lib/jquery-unobtrusive-ajax/jquery.unobtrusive-ajax.js"></script>
    
    <script>
        $(document).ready(function () {
            $('#tableProduzione').DataTable({ "scrollX": true });
        });

        function OnSuccessRequest() {
            $('#tableProduzione').DataTable({});
            document.getElementById('Scatole_Scatola').value = '';
            document.getElementById('Scatole_Note').value = '';
        };
    </script>
}
```

#### ? DOPO (Fetch API)

```razor
<form method="post" id="formInsertScatola">
    <input asp-for="Scatole.DataSorter" class="form-control" />
    <textarea asp-for="Scatole.Note" class="form-control"></textarea>
    <input asp-for="Scatole.Scatola" class="form-control" />
    
    <button type="submit" class="btn btn-primary">Save</button>
</form>

@section scripts {
    <script type="module">
        import { FetchHelpers } from '/js/fetch-helpers.js';

        // Inizializza DataTable al caricamento
        $(document).ready(function () {
            $('#tableProduzione').DataTable({ scrollX: true });
        });

        // Handler submit form
        document.getElementById('formInsertScatola').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const form = e.target;
            const formData = new FormData(form);
            
            try {
                await FetchHelpers.postFormWithUpdate(
                    '?handler=InsertScatola',
                    formData,
                    'resultDiv',
                    () => {
                        // Success callback
                        FetchHelpers.reinitDataTable('tableProduzione');
                        FetchHelpers.clearInputs(['Scatole_Scatola', 'Scatole_Note']);
                    }
                );
            } catch (error) {
                console.error('Errore inserimento scatola:', error);
            }
        });
    </script>
}
```

**?? Modifiche:**
- ? Rimosso `data-ajax="true"` e relativi attributi
- ? Rimosso `jquery.unobtrusive-ajax.js`
- ? Aggiunto `id="formInsertScatola"`
- ? Usato `<button type="submit">` invece di `<input type="submit">`
- ? Handler `submit` con Fetch API
- ? Import module ES6
- ? Callback successo con helper methods

---

### Esempio 2: PagesNormalizzato/Index.cshtml (Con Spinner)

#### ? PRIMA

```razor
<form method="post"
      data-ajax="true"
      data-ajax-method="post"
      data-ajax-update="#dvReport"
      data-ajax-mode="replace"
      data-ajax-url="?handler=Report"
      data-ajax-success="OnSuccessRequest">
    
    <select asp-for="SelectedCentro" asp-items="Model.LstCentri"></select>
    <input asp-for="StartDate" class="form-control" />
    <input asp-for="EndDate" class="form-control" />
    
    <input type="submit" value="Report" class="btn btn-primary" />
</form>

<div id="divProcessing" style="display:none;">
    <img src="~/images/Spinner.gif" />
</div>

<div id="dvReport"></div>

@section scripts {
    <script src="~/lib/jquery-unobtrusive-ajax/jquery.unobtrusive-ajax.js"></script>
    <script>
        $("#divProcessing").hide();
        
        function OnSuccessRequest() {
            $('#tableProduzione').DataTable({ "scrollX": true });
        }
    </script>
}
```

#### ? DOPO

```razor
<form method="post" id="formReport">
    <select asp-for="SelectedCentro" asp-items="Model.LstCentri"></select>
    <input asp-for="StartDate" class="form-control" />
    <input asp-for="EndDate" class="form-control" />
    
    <button type="submit" class="btn btn-primary">Report</button>
</form>

<div id="divProcessing" style="display:none;">
    <p class="text-center">Processing data, please wait...</p>
    <img class="mx-auto d-block" src="~/images/Spinner.gif" />
</div>

<div id="dvReport"></div>

@section scripts {
    <script type="module">
        import { FetchHelpers } from '/js/fetch-helpers.js';

        // Nascondi spinner all'avvio
        FetchHelpers.hideSpinner('divProcessing');

        document.getElementById('formReport').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const form = e.target;
            const formData = new FormData(form);
            
            // Mostra spinner
            FetchHelpers.showSpinner('divProcessing');
            
            try {
                await FetchHelpers.postFormWithUpdate(
                    '?handler=Report',
                    formData,
                    'dvReport',
                    () => {
                        // Success: nascondi spinner e reinit DataTable
                        FetchHelpers.hideSpinner('divProcessing');
                        FetchHelpers.reinitDataTable('tableProduzione', { scrollX: true });
                    },
                    (error) => {
                        // Error: nascondi spinner e mostra messaggio
                        FetchHelpers.hideSpinner('divProcessing');
                        alert('Errore durante il caricamento del report. Riprova.');
                    }
                );
            } catch (error) {
                console.error('Errore report:', error);
                FetchHelpers.hideSpinner('divProcessing');
            }
        });
    </script>
}
```

---

### Esempio 3: PagesAccettazione/Create.cshtml (Form Multipli)

#### ? PRIMA

```razor
<!-- Form 1: Inserimento Dispacci -->
<form method="post"
      data-ajax="true"
      data-ajax-method="post"
      data-ajax-update="#resultDivDispacci"
      data-ajax-mode="replace"
      data-ajax-url="?handler=InsertDispacci"
      data-ajax-success="OnSuccessRequestDispacci">
    <input asp-for="Dispaccio" class="form-control" />
    <input type="submit" value="Add" class="btn btn-primary" />
</form>

<div id="resultDivDispacci">
    <partial name="_RiepilogoDispacci" model="Model.DispacciModel" />
</div>

<!-- Form 2: Crea Bancale -->
<form method="post"
      data-ajax="true"
      data-ajax-method="post"
      data-ajax-update="#resultDiv"
      data-ajax-mode="replace"
      data-ajax-url="?handler=CreaNuovoBancale"
      data-ajax-success="OnSuccessRequest">
    <input asp-for="Bancali.DataAccettazioneBancale" class="form-control" />
    <select asp-for="Bancali.IdCommessa" asp-items="@Model.CommesseSL"></select>
    <textarea asp-for="Bancali.Note" class="form-control"></textarea>
    <input type="submit" value="Nuovo bancale" class="btn btn-primary" />
</form>

<div id="resultDiv">
    <partial name="_RiepilogoAccettazioneBancali" model="Model.BancaliModel" />
</div>
```

#### ? DOPO

```razor
<!-- Form 1: Inserimento Dispacci -->
<form method="post" id="formInsertDispacci">
    <input asp-for="Dispaccio" class="form-control" />
    <button type="submit" class="btn btn-primary">Add</button>
</form>

<div id="resultDivDispacci">
    <partial name="_RiepilogoDispacci" model="Model.DispacciModel" />
</div>

<!-- Form 2: Crea Bancale -->
<form method="post" id="formCreaNuovoBancale">
    <input asp-for="Bancali.DataAccettazioneBancale" class="form-control" />
    <select asp-for="Bancali.IdCommessa" asp-items="@Model.CommesseSL"></select>
    <textarea asp-for="Bancali.Note" class="form-control"></textarea>
    <button type="submit" class="btn btn-primary">Nuovo bancale</button>
</form>

<div id="resultDiv">
    <partial name="_RiepilogoAccettazioneBancali" model="Model.BancaliModel" />
</div>

@section scripts {
    <script type="module">
        import { FetchHelpers } from '/js/fetch-helpers.js';

        // Inizializza DataTables
        $(document).ready(function () {
            $('#tableProduzione').DataTable({ scrollX: true });
            $('#tableProduzioneDispacci').DataTable({
                scrollY: '20vh',
                scrollCollapse: true,
                paging: false,
                dom: '<<t>i>'
            });
        });

        // Handler Form 1: Inserimento Dispacci
        document.getElementById('formInsertDispacci').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = new FormData(e.target);
            
            try {
                await FetchHelpers.postFormWithUpdate(
                    '?handler=InsertDispacci',
                    formData,
                    'resultDivDispacci',
                    () => {
                        // Success
                        FetchHelpers.clearInputs(['Dispaccio']);
                        FetchHelpers.reinitDataTable('tableProduzioneDispacci', {
                            scrollY: '20vh',
                            scrollCollapse: true,
                            paging: false,
                            dom: '<<t>i>'
                        });
                    }
                );
            } catch (error) {
                console.error('Errore inserimento dispaccio:', error);
            }
        });

        // Handler Form 2: Crea Nuovo Bancale
        document.getElementById('formCreaNuovoBancale').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = new FormData(e.target);
            
            try {
                await FetchHelpers.postFormWithUpdate(
                    '?handler=CreaNuovoBancale',
                    formData,
                    'resultDiv',
                    () => {
                        // Success
                        FetchHelpers.clearInputs(['Dispaccio', 'Bancali_Note']);
                        FetchHelpers.reinitDataTable('tableProduzione', { 
                            scrollX: true, 
                            order: [] 
                        });
                        FetchHelpers.dataTableGoToLastPage('tableProduzione');
                        
                        // Pulisci tabella dispacci
                        const table = $('#tableProduzioneDispacci').DataTable();
                        table.clear().draw();
                    }
                );
            } catch (error) {
                console.error('Errore creazione bancale:', error);
            }
        });
    </script>
}
```

---

### Esempio 4: PagesNormalizzazione/Create.cshtml (Vue.js + Axios)

**?? Nota**: Questo file usa Vue.js per gestione stato + Axios per chiamate GET. Solo il form submit usa jQuery Ajax.

#### ? PRIMA (Parte Axios da mantenere)

```javascript
// Chiamate Axios esistenti (DA MANTENERE)
axios({
    method: 'get',
    headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
    url: '?handler=AssociazioneTipologia',
    params: { idCommessa: Number(self.selectCommessa) }
})
.then(function (response) {
    var obj = JSON.parse(response.data);
    // ... gestione risposta
})
```

#### ? DOPO (Migrazione solo form submit)

```razor
<form method="post" id="formInsertScatola">
    <!-- campi form Vue.js invariati -->
    <button type="submit" class="btn btn-primary" :disabled="isFormInvalid">Save</button>
</form>

@section scripts {
    <script type="module">
        import { FetchHelpers } from '/js/fetch-helpers.js';

        $(document).ready(function () {
            $('#tableProduzione').DataTable({ scrollX: true });
        });

        // Handler form submit
        document.getElementById('formInsertScatola').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = new FormData(e.target);
            
            try {
                await FetchHelpers.postFormWithUpdate(
                    '?handler=InsertScatola',
                    formData,
                    'resultDiv',
                    () => {
                        FetchHelpers.reinitDataTable('tableProduzione', { scrollX: true });
                        FetchHelpers.dataTableGoToLastPage('tableProduzione');
                        
                        // Reset campi Vue (usa i metodi Vue esistenti)
                        document.getElementById('Scatole_Scatola').value = '';
                        document.getElementById('Scatole_Note').value = '';
                    }
                );
            } catch (error) {
                console.error('Errore inserimento scatola:', error);
            }
        });

        // Vue.js instance INVARIATA
        var vm = new Vue({
            el: '#app',
            data: { /* ... */ },
            methods: {
                // Metodi Axios INVARIATI
                onChangeCommessa: function (event) {
                    axios({
                        method: 'get',
                        headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                        url: '?handler=AssociazioneTipologia',
                        params: { idCommessa: Number(this.selectCommessa) }
                    })
                    .then(function (response) {
                        // ... gestione invariata
                    });
                }
                // ... altri metodi
            }
        });
    </script>
}
```

**?? Note**:
- ? Vue.js instance rimane INVARIATA
- ? Chiamate Axios rimangono INVARIATE (opzionale migrare dopo)
- ? Solo form submit migrato a Fetch API

---

## Pattern Comuni

### Pattern 1: Form Submit con DataTable Refresh

```javascript
document.getElementById('formId').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    
    await FetchHelpers.postFormWithUpdate(
        '?handler=HandlerName',
        formData,
        'resultDivId',
        () => {
            FetchHelpers.reinitDataTable('tableId');
            FetchHelpers.clearInputs(['input1', 'input2']);
        }
    );
});
```

### Pattern 2: Form Submit con Spinner

```javascript
document.getElementById('formId').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    FetchHelpers.showSpinner('spinnerId');
    
    const formData = new FormData(e.target);
    
    try {
        await FetchHelpers.postFormWithUpdate(
            '?handler=HandlerName',
            formData,
            'resultDivId',
            () => FetchHelpers.hideSpinner('spinnerId'),
            () => FetchHelpers.hideSpinner('spinnerId')
        );
    } catch (error) {
        FetchHelpers.hideSpinner('spinnerId');
    }
});
```

### Pattern 3: Form Submit + Vai Ultima Pagina DataTable

```javascript
await FetchHelpers.postFormWithUpdate(
    '?handler=HandlerName',
    formData,
    'resultDivId',
    () => {
        FetchHelpers.reinitDataTable('tableId', { scrollX: true, order: [] });
        FetchHelpers.dataTableGoToLastPage('tableId');
        FetchHelpers.clearInputs(['input1']);
    }
);
```

### Pattern 4: Form Multipli nella Stessa Pagina

```javascript
// Form 1
document.getElementById('form1').addEventListener('submit', async (e) => {
    e.preventDefault();
    await FetchHelpers.postFormWithUpdate(
        '?handler=Handler1',
        new FormData(e.target),
        'resultDiv1',
        onSuccessForm1
    );
});

// Form 2
document.getElementById('form2').addEventListener('submit', async (e) => {
    e.preventDefault();
    await FetchHelpers.postFormWithUpdate(
        '?handler=Handler2',
        new FormData(e.target),
        'resultDiv2',
        onSuccessForm2
    );
});
```

---

## Checklist Implementazione

### ? Fase 1: Setup Iniziale

- [ ] **Creare `wwwroot/js/fetch-helpers.js`**
  - Copiare codice helper completo
  - Testare import in una pagina

- [ ] **Testare compatibilità browser**
  - Chrome (>= 42)
  - Firefox (>= 39)
  - Edge (>= 14)
  - Safari (>= 10.1)

### ? Fase 2: Migrazione File (Priorità 1)

- [ ] **PagesSorter/Create.cshtml** (?? Semplice)
  - Migrare form InsertScatola
  - Testare submit + DataTable refresh
  - Rimuovere `jquery.unobtrusive-ajax.js`

- [ ] **PagesSpostaGiacenza/Create.cshtml** (?? Semplice)
  - Migrare form AggiornaScatola
  - Testare submit + DataTable refresh
  - Rimuovere `jquery.unobtrusive-ajax.js`

- [ ] **PagesAccettazione/Create.cshtml** (?? Media)
  - Migrare form InsertDispacci
  - Migrare form CreaNuovoBancale
  - Testare interazione tra 2 form
  - Testare 2 DataTables
  - Rimuovere `jquery.unobtrusive-ajax.js`

- [ ] **PagesNormalizzazione/Create.cshtml** (?? Complessa)
  - Migrare solo form submit
  - Mantenere Vue.js + Axios invariati
  - Testare compatibilità Vue.js
  - Rimuovere `jquery.unobtrusive-ajax.js`

### ? Fase 3: Migrazione Report (Priorità 2)

- [ ] **PagesNormalizzato/Index.cshtml**
  - Migrare form Report
  - Testare spinner show/hide
  - Rimuovere `jquery.unobtrusive-ajax.js`

- [ ] **PagesMacero/Index.cshtml**
  - Migrare form ReportNormalizzazione
  - Testare spinner show/hide
  - Rimuovere `jquery.unobtrusive-ajax.js`

### ? Fase 4: Cleanup

- [ ] **TipologiaNormalizzazione/Index.cshtml**
  - Rimuovere solo include `jquery.unobtrusive-ajax.js`
  - Nessuna migrazione necessaria

- [ ] **Aggiornare _Layout.cshtml**
  - Verificare jQuery ancora necessario (Bootstrap, DataTables)
  - Rimuovere `jquery.unobtrusive-ajax.js` se presente

- [ ] **Verificare tutti gli @section scripts**
  - Cercare altre pagine con `data-ajax="true"`
  - Usare ricerca globale: `data-ajax="true"`

### ? Fase 5: Testing Completo

- [ ] **Test funzionali**
  - Submit form con dati validi
  - Submit form con dati invalidi (validation)
  - Refresh DataTables
  - Spinner loading states
  - Clear input dopo submit

- [ ] **Test integrazione**
  - Form multipli nella stessa pagina
  - Interazione tra form
  - Vue.js + Fetch API compatibilità

- [ ] **Test performance**
  - Tempo caricamento pagina (ridotto ~85KB)
  - Tempo risposta submit form
  - Network tab (verifica requests)

- [ ] **Test browser**
  - Chrome
  - Firefox
  - Edge
  - Safari (se necessario)

---

## Testing

### Test Manuale - Checklist per ogni Form

```
Form: ____________________ Pagina: ____________________

? Submit form con dati validi
   ? Risposta HTML ricevuta
   ? Elemento DOM aggiornato correttamente
   ? DataTable reinizializzata (se presente)

? Submit form con dati invalidi
   ? Messaggi validation visualizzati
   ? Form non submitta

? Spinner (se presente)
   ? Mostra durante submit
   ? Nasconde dopo risposta

? Clear input (se previsto)
   ? Input puliti dopo submit successo

? DataTable (se presente)
   ? Reinizializzata correttamente
   ? Paginazione funzionante
   ? Ultima pagina visualizzata (se previsto)

? Nessun errore console JavaScript

? Nessun errore Network (4xx, 5xx)
```

### Test Automatizzati (Opzionale)

```javascript
// Esempio test con Playwright
test('Submit form inserimento scatola', async ({ page }) => {
    await page.goto('/PagesSorter/Create');
    
    await page.fill('#Scatole_DataSorter', '2025-01-15');
    await page.fill('#Scatole_Scatola', 'TEST123');
    
    await page.click('button[type="submit"]');
    
    // Attendi risposta
    await page.waitForSelector('#resultDiv table');
    
    // Verifica DataTable
    const rows = await page.locator('#tableProduzione tbody tr').count();
    expect(rows).toBeGreaterThan(0);
});
```

---

## ?? Riepilogo Finale

### Vantaggi Migrazione

? **Performance**: -85KB bundle size  
? **Manutenibilità**: Standard web nativo  
? **Sicurezza**: Nessuna dipendenza esterna obsoleta  
? **Modernità**: Async/await ES6+  
? **Developer Experience**: Debugging più semplice

### Tempo Stimato per File

| File | Complessità | Tempo | Note |
|------|-------------|-------|------|
| PagesSorter/Create | ?? Bassa | 30 min | Form semplice |
| PagesSpostaGiacenza/Create | ?? Bassa | 30 min | Form semplice |
| PagesNormalizzato/Index | ?? Bassa | 30 min | Form con spinner |
| PagesMacero/Index | ?? Bassa | 30 min | Form con spinner |
| PagesAccettazione/Create | ?? Media | 1 ora | 2 form, 2 DataTables |
| PagesNormalizzazione/Create | ?? Alta | 1.5 ore | Vue.js + Axios |
| TipologiaNormalizzazione/Index | ?? Bassa | 5 min | Solo cleanup |
| **TOTALE** | | **~5 ore** | + 1 ora testing |

### Risorse

?? **Documentazione**:
- [Fetch API - MDN](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API)
- [FormData - MDN](https://developer.mozilla.org/en-US/docs/Web/API/FormData)
- [ES6 Modules - MDN](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules)

??? **Tools**:
- Chrome DevTools Network Tab
- VS Code Extension: ES6 Syntax Highlighting

---

**Ultima modifica**: 2025-01-XX  
**Versione**: 1.0  
**Autore**: GitHub Copilot  
**Stato**: ? Completo e pronto per implementazione

---

?? **Buon refactoring!**
