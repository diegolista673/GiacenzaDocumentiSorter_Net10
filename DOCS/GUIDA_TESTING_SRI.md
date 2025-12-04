# ?? Guida Rapida Testing Hash SRI

**Tempo Stimato:** 10 minuti  
**Prerequisito:** Fix hash SRI applicati

---

## ?? Obiettivo

Verificare che tutti gli hash SRI siano corretti e che le librerie CDN si carichino senza errori.

---

## ?? Checklist Testing (10 passi)

### 1?? Fermare Debug (se attivo)

**Visual Studio:**
- Premi `Shift + F5`
- Oppure: Menu ? Debug ? Stop Debugging

**VS Code:**
- Premi `Shift + F5`
- Oppure: Pannello Run ? Stop

---

### 2?? Riavviare Applicazione

**Visual Studio:**
- Premi `F5`
- Oppure: Menu ? Debug ? Start Debugging

**VS Code:**
- Premi `F5`
- Oppure: Pannello Run ? Start Debugging

**Kestrel Manuale:**
```bash
cd C:\Users\SMARTW\source\repos\GiacenzaSorter
dotnet run
```

**Attendi:** "Now listening on: http://localhost:5000"

---

### 3?? Aprire Browser e DevTools

1. **Apri Chrome/Edge:**
   - URL: `http://localhost:5000` (o porta configurata)

2. **Apri Console (F12):**
   - Premi `F12`
   - Vai al tab **Console**
   - Vai al tab **Network** (prepara per prossimo step)

---

### 4?? Ricaricare Pagina (Cache Disabilitata)

**Importante:** Cache disabilitata forza scaricamento da CDN

**Windows:**
- `Ctrl + Shift + R`
- Oppure: `Ctrl + F5`

**Mac:**
- `Cmd + Shift + R`

**Manuale:**
1. F12 ? Network tab
2. Spunta "Disable cache"
3. Ricarica pagina normale (F5)

---

### 5?? Verificare Console - Zero Errori SRI

**Console Tab - Cosa cercare:**

? **SUCCESSO - Nessun errore:**
```
[Empty console or only info messages]
```

? **ERRORE - Hash non corrispondente:**
```
Failed to find a valid digest in the 'integrity' attribute for resource...
The resource has been blocked.
```

**Se vedi errori:**
1. Copia messaggio completo
2. Identifica libreria problematica
3. Esegui `.\verify-sri-hashes.ps1`
4. Aggiorna hash con `.\update-sri-hashes.ps1`

---

### 6?? Verificare Network Tab - Tutte le Risorse 200 OK

**Network Tab - Filtri:**
1. Filtra per: `css, js`
2. Ordina per: `Status`

**Verifica ogni libreria CDN:**

| Libreria | URL | Status Atteso | Size |
|----------|-----|---------------|------|
| jQuery | code.jquery.com | 200 | ~85 KB |
| jQuery UI CSS | cdnjs...jqueryui | 200 | ~30 KB |
| jQuery UI JS | cdnjs...jqueryui | 200 | ~250 KB |
| Bootstrap CSS | cdn.jsdelivr.net/bootstrap | 200 | ~160 KB |
| Bootstrap JS | cdn.jsdelivr.net/bootstrap | 200 | ~60 KB |
| Font Awesome | cdnjs...font-awesome | 200 | ~75 KB |
| DataTables CSS | cdn.datatables.net | 200 | ~15 KB |
| DataTables JS | cdn.datatables.net | 200 | ~200 KB |
| JSZip | cdnjs...jszip | 200 | ~110 KB |
| Vue.js | cdn.jsdelivr.net/vue | 200 | ~90 KB |
| Axios | cdn.jsdelivr.net/axios | 200 | ~15 KB |
| Moment.js | cdnjs...moment | 200 | ~55 KB |
| jQuery Validate | cdnjs...jquery-validate | 200 | ~25 KB |

**Se Status ? 200:**
- **304 (Not Modified):** OK, da cache
- **404 (Not Found):** URL errato, verifica _Layout.cshtml
- **403 (Forbidden):** Firewall/proxy blocca CDN
- **ERR_BLOCKED_BY_CLIENT:** AdBlock o estensione blocca
- **net::ERR_CERT_*:** Problema SSL/TLS

---

### 7?? Test Funzionale: jQuery UI DatePicker

**Pagina:** `/PagesNormalizzazione/Create`

1. **Naviga:** Menu ? Logistica ? Normalizzazione Scatole
2. **Trova campo:** "Data Normalizzazione"
3. **Click sul campo:** Dovrebbe apparire calendario popup
4. **Seleziona data:** Click su giorno qualsiasi
5. **Verifica:** Data inserita nel campo

? **SUCCESSO:** Calendario appare e data si inserisce  
? **ERRORE:** Nessun calendario ? jQuery UI non caricata

---

### 8?? Test Funzionale: DataTables + JSZip (Export Excel)

**Pagina:** `/PagesRiepilogo/Index` (o qualsiasi report)

1. **Naviga:** Menu ? Reports ? Report Scatole
2. **Compila filtri:** Data inizio/fine
3. **Click "Report":** Attendi caricamento tabella
4. **Verifica tabella:** DataTable con paginazione/ordinamento
5. **Click "Copy to Excel":** Scarica file .xlsx
6. **Apri Excel:** Verifica dati presenti

? **SUCCESSO:** Export Excel funziona  
? **ERRORE:** Export fallisce ? JSZip non caricato

---

### 9?? Test Funzionale: Vue.js (se usato)

**Pagina:** `/PagesNormalizzazione/Create`

1. **Naviga:** Normalizzazione Scatole
2. **Seleziona Bancale:** Dropdown dovrebbe popolarsi dinamicamente
3. **Verifica:** Componenti Vue.js reagiscono

? **SUCCESSO:** Dropdown dinamici funzionano  
? **ERRORE:** Dropdown vuoti ? Vue.js non caricato

---

### ?? Test Funzionale: Axios (API calls)

**Pagina:** `/PagesNormalizzazione/Create`

1. **Apri Console:** F12 ? Network ? XHR
2. **Seleziona Bancale:** Dropdown trigger
3. **Verifica chiamata API:** Appare in Network tab
4. **Status:** 200 OK
5. **Response:** JSON con dati

? **SUCCESSO:** Chiamate API completate  
? **ERRORE:** Nessuna chiamata ? Axios non funziona

---

## ? Riepilogo Test

| Test | Descrizione | Status |
|------|-------------|--------|
| **Console** | 0 errori SRI | ? |
| **Network** | Tutte risorse 200 OK | ? |
| **DatePicker** | jQuery UI funzionante | ? |
| **Export Excel** | JSZip funzionante | ? |
| **Vue.js** | Componenti dinamici | ? |
| **Axios** | API calls funzionanti | ? |

**Se tutti ?:** Deploy pronto!  
**Se qualcuno ?:** Vedi troubleshooting sotto

---

## ?? Troubleshooting

### Problema: Errore SRI ancora presente

**Soluzione:**
```powershell
# 1. Verifica hash corretti
.\verify-sri-hashes.ps1

# 2. Confronta output con _Layout.cshtml
# Se diversi, aggiorna:
.\update-sri-hashes.ps1

# 3. Riavvia applicazione
# 4. Riprova test
```

---

### Problema: Libreria non si carica (Status ? 200)

**Causa 1: Firewall/Proxy blocca CDN**
```
Soluzione: Contatta IT per whitelist CDN:
- cdnjs.cloudflare.com
- cdn.jsdelivr.net
- code.jquery.com
- cdn.datatables.net
```

**Causa 2: AdBlock blocca risorse**
```
Soluzione: Disabilita AdBlock su localhost
1. Click icona AdBlock
2. "Don't run on pages on this domain"
3. Ricarica pagina
```

**Causa 3: URL errato in _Layout.cshtml**
```
Soluzione: Verifica URL in _Layout.cshtml
- Copia URL da browser
- Confronta con _Layout.cshtml
- Correggi se diverso
```

---

### Problema: DatePicker non appare

**Verifica:**
1. Console ? Errori JavaScript?
2. Network ? jquery-ui.min.js caricato?
3. Network ? jquery-ui.min.css caricato?

**Soluzione:**
```javascript
// Console ? digita:
typeof jQuery
// Output atteso: "function"

typeof $.datepicker
// Output atteso: "function"

// Se "undefined" ? libreria non caricata
```

---

### Problema: Export Excel fallisce

**Verifica:**
```javascript
// Console ? digita:
typeof JSZip
// Output atteso: "function"

// Se "undefined" ? JSZip non caricato
```

**Soluzione:**
- Verifica ordine script in _Layout.cshtml
- JSZip DEVE essere prima di buttons.html5.min.js
- Verifica hash SRI per JSZip

---

### Problema: Vue.js non funziona

**Verifica:**
```javascript
// Console ? digita:
typeof Vue
// Output atteso: "function"

// Se "undefined" ? Vue.js non caricato
```

**Soluzione:**
- Verifica hash SRI per Vue.js
- Verifica versione corretta (2.7.16)

---

### Problema: Axios API calls falliscono

**Verifica:**
```javascript
// Console ? digita:
typeof axios
// Output atteso: "function"

// Se "undefined" ? Axios non caricato
```

**Soluzione:**
- Verifica hash SRI per Axios
- Verifica versione fissa (@1.6.7)
- Non usare `/dist/axios.min.js` senza versione

---

## ?? Log Testing

**Copia questo template e compila durante test:**

```markdown
## Test SRI Hash - [DATA]

### Ambiente
- Browser: [Chrome/Edge/Firefox] [Versione]
- URL: http://localhost:[PORTA]
- Build: [Success/Fail]

### Console Errors
- [ ] ? Nessun errore SRI
- [ ] ?? Errori presenti: [DESCRIZIONE]

### Network Status
- [ ] ? jQuery (200 OK)
- [ ] ? jQuery UI CSS (200 OK)
- [ ] ? jQuery UI JS (200 OK)
- [ ] ? Bootstrap CSS (200 OK)
- [ ] ? Bootstrap JS (200 OK)
- [ ] ? Font Awesome (200 OK)
- [ ] ? JSZip (200 OK)
- [ ] ? Vue.js (200 OK)
- [ ] ? Axios (200 OK)
- [ ] ? Moment.js (200 OK)
- [ ] ? jQuery Validate (200 OK)

### Functional Tests
- [ ] ? DatePicker funziona
- [ ] ? Export Excel funziona
- [ ] ? Vue.js componenti OK
- [ ] ? Axios API calls OK

### Esito Finale
- [ ] ? PASS - Tutti i test superati
- [ ] ? FAIL - [DESCRIZIONE PROBLEMA]

### Note
[Eventuali osservazioni o problemi riscontrati]
```

---

## ?? Post-Testing Actions

### Se TUTTI i test sono ?

1. **Commit modifiche:**
```bash
git add Pages/Shared/_Layout.cshtml
git add verify-sri-hashes.ps1
git add update-sri-hashes.ps1
git add DOCS/*.md
git commit -m "fix: Corretti hash SRI per jQuery UI CSS, Axios, JSZip, Vue.js"
git push origin master
```

2. **Deploy in ambiente test:**
   - Segui procedura deploy specifica progetto
   - Ripeti testing in ambiente test
   - Se OK ? Deploy produzione

3. **Monitoraggio post-deploy:**
   - Console browser produzione (0 errori)
   - Logs applicazione (nessun errore caricamento librerie)
   - Metriche performance (tempo caricamento pagina)

---

### Se QUALCHE test è ?

1. **NON committare**
2. **Analizza problema** (vedi Troubleshooting)
3. **Risolvi issue**
4. **Riprova testing**
5. **Commit solo quando tutto ?**

---

## ?? Tempo Stimato per Test

| Test | Tempo |
|------|-------|
| **Console Check** | 1 min |
| **Network Check** | 2 min |
| **DatePicker** | 1 min |
| **Export Excel** | 2 min |
| **Vue.js** | 1 min |
| **Axios** | 1 min |
| **Troubleshooting** (se necessario) | 5-10 min |
| **TOTALE** | **8-18 min** |

---

## ?? Checklist Rapida

```
? Debug fermato e applicazione riavviata
? Browser aperto con DevTools (F12)
? Pagina ricaricata con cache disabilitata (Ctrl+Shift+R)
? Console: 0 errori SRI
? Network: Tutte risorse 200 OK
? DatePicker: Funzionante
? Export Excel: Funzionante
? Vue.js: Componenti OK
? Axios: API calls OK
? Log test compilato
? Commit eseguito (se tutti ?)
```

---

**Buon Testing!** ???

---

**Autore**: GitHub Copilot  
**Data**: 2025-01-24  
**Versione**: 1.0
