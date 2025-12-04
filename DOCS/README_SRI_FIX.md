# ?? Documentazione Fix Hash SRI - Indice

**Progetto**: GiacenzaSorterRm  
**Data**: 2025-01-24  
**Versione**: 1.0

---

## ?? File Documentazione

### 1. ?? FIX_SRI_HASH_JQUERY_UI.md

**Descrizione**: Guida completa al fix degli hash SRI  
**Contenuto**:
- Spiegazione dettagliata problema
- Soluzioni applicate per ogni libreria
- Cos'è SRI e come funziona
- Come generare hash SRI
- Troubleshooting
- Best practices

**Quando usare**: Per capire cosa è stato fatto e perché

---

### 2. ?? RIEPILOGO_FIX_SRI_SESSIONE.md

**Descrizione**: Riepilogo esecutivo della sessione di fix  
**Contenuto**:
- Problemi risolti (jQuery UI CSS, Axios)
- Hash aggiunti (JSZip, Vue.js)
- Statistiche sicurezza prima/dopo
- Checklist completamento
- Deployment checklist

**Quando usare**: Per report management o documentazione progetto

---

### 3. ?? GUIDA_TESTING_SRI.md

**Descrizione**: Guida passo-passo per testing manuale  
**Contenuto**:
- 10 passi testing dettagliati
- Screenshots e istruzioni chiare
- Troubleshooting per ogni test
- Template log testing
- Tempo stimato per test

**Quando usare**: Quando devi testare manualmente dopo modifiche

---

### 4. ?? CLEANUP_LIBRERIE_CDN_REPORT.md

**Descrizione**: Report pulizia librerie locali wwwroot/lib  
**Contenuto**:
- Analisi librerie locali vs CDN
- Rimozione 103 file (10.47 MB)
- Verifica integrità CDN
- Script aggiornamento hash

**Quando usare**: Per riferimento su pulizia effettuata

---

## ?? Script PowerShell

### 1. verify-sri-hashes.ps1

**Funzione**: Verifica automatica hash SRI  
**Utilizzo**:
```powershell
.\verify-sri-hashes.ps1
```

**Output**:
- Scarica ogni libreria CDN
- Calcola hash SHA-256/384/512
- Genera snippet HTML
- Confronta con _Layout.cshtml
- Salva risultati JSON

**Quando usare**:
- Prima di ogni deploy
- Dopo aggiornamento librerie
- Durante code review
- Setup CI/CD

---

### 2. update-sri-hashes.ps1

**Funzione**: Aggiorna automaticamente hash SRI errati  
**Utilizzo**:
```powershell
# Test (no modifiche)
.\update-sri-hashes.ps1 -WhatIf

# Applicazione modifiche
.\update-sri-hashes.ps1
```

**Output**:
- Backup automatico _Layout.cshtml
- Aggiorna hash non corrispondenti
- Aggiunge hash mancanti
- Istruzioni post-update

**Quando usare**:
- Dopo aver eseguito verify-sri-hashes.ps1
- Quando hash non corrispondono
- Per automazione update

---

### 3. test-sri-automated.ps1

**Funzione**: Test automatico hash SRI su applicazione in esecuzione  
**Utilizzo**:
```powershell
# Default (localhost:5000)
.\test-sri-automated.ps1

# Porta custom
.\test-sri-automated.ps1 -BaseUrl "http://localhost:5001"
```

**Output**:
- Verifica applicazione raggiungibile
- Verifica presenza librerie in HTML
- Verifica attributi integrity
- Test accessibilità CDN
- Confronto hash con file locale

**Quando usare**:
- Dopo modifiche _Layout.cshtml
- Prima di commit
- Testing automatizzato CI/CD

---

## ?? Quick Start

### Scenario 1: Ho appena fatto modifiche a _Layout.cshtml

```powershell
# 1. Verifica hash
.\verify-sri-hashes.ps1

# 2. Se hash non corrispondono, aggiorna
.\update-sri-hashes.ps1

# 3. Riavvia applicazione
dotnet run

# 4. Test automatico
.\test-sri-automated.ps1

# 5. Test manuale (vedi GUIDA_TESTING_SRI.md)

# 6. Se tutto OK, commit
git add .
git commit -m "fix: Aggiornati hash SRI"
git push
```

---

### Scenario 2: Voglio aggiornare una libreria CDN

```powershell
# 1. Modifica _Layout.cshtml (cambia versione libreria)
# Esempio: vue@2.7.16 ? vue@3.0.0

# 2. Calcola nuovo hash
.\verify-sri-hashes.ps1

# 3. Copia hash generato da output
# 4. Incolla in _Layout.cshtml nell'attributo integrity

# 5. Test
.\test-sri-automated.ps1

# 6. Test manuale (vedi GUIDA_TESTING_SRI.md)

# 7. Commit
git commit -m "chore: Aggiornato Vue.js a 3.0.0"
```

---

### Scenario 3: Voglio aggiungere una nuova libreria CDN

```powershell
# 1. Aggiungi libreria in _Layout.cshtml SENZA hash
# Esempio:
# <script src="https://cdn.example.com/lib.min.js"></script>

# 2. Modifica verify-sri-hashes.ps1
# Aggiungi URL nella lista $cdnResources

# 3. Esegui script
.\verify-sri-hashes.ps1

# 4. Copia snippet HTML generato (con hash)

# 5. Incolla in _Layout.cshtml (sostituisce riga senza hash)

# 6. Test
.\test-sri-automated.ps1

# 7. Commit
git commit -m "feat: Aggiunta libreria XYZ con hash SRI"
```

---

### Scenario 4: Errore SRI in produzione

```powershell
# 1. Identifica libreria problematica (console browser)
# Errore: "Failed to find a valid digest... for resource https://cdn.example.com/lib.js"

# 2. Scarica file locale per debug
Invoke-WebRequest -Uri "https://cdn.example.com/lib.js" -OutFile "lib-debug.js"

# 3. Calcola hash locale
.\verify-sri-hashes.ps1

# 4. Confronta con _Layout.cshtml

# 5. Aggiorna hash
.\update-sri-hashes.ps1

# 6. Deploy hotfix
git commit -m "hotfix: Corretto hash SRI per libreria XYZ"
git push
```

---

## ?? Checklist Pre-Deploy

```
? Build compilata senza errori
? verify-sri-hashes.ps1 eseguito (tutti hash corretti)
? test-sri-automated.ps1 eseguito (PASS)
? Testing manuale completato (GUIDA_TESTING_SRI.md)
? Console browser: 0 errori SRI
? Network tab: tutte risorse 200 OK
? DatePicker funzionante
? Export Excel funzionante
? Vue.js componenti OK
? Axios API calls OK
? Cross-browser testing (Chrome, Firefox, Edge)
? Documentazione aggiornata
? Commit con messaggio descrittivo
? Push su repository
```

---

## ?? Supporto e Troubleshooting

### Problema: Script PowerShell non si avvia

**Errore**:
```
.\verify-sri-hashes.ps1 : File cannot be loaded because running scripts is disabled
```

**Soluzione**:
```powershell
# Abilita esecuzione script (come amministratore)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Riprova script
.\verify-sri-hashes.ps1
```

---

### Problema: Hash calcolato diverso da quello nel browser

**Causa**: CDN ha aggiornato file o compressione diversa

**Soluzione**:
1. Copia hash dall'errore browser (è quello corretto!)
2. Aggiorna _Layout.cshtml con hash browser
3. Oppure: usa versione specifica libreria (`@1.2.3`)

---

### Problema: CDN non raggiungibile

**Causa**: Firewall/proxy aziendale blocca CDN

**Soluzione temporanea**:
```powershell
# Test connettività
Test-NetConnection cdnjs.cloudflare.com -Port 443

# Se fallisce, contatta IT per whitelist:
# - cdnjs.cloudflare.com
# - cdn.jsdelivr.net
# - code.jquery.com
```

**Soluzione permanente**: Self-hosting librerie (annulla benefici CDN)

---

## ?? Link Rapidi

- [MDN - Subresource Integrity](https://developer.mozilla.org/en-US/docs/Web/Security/Subresource_Integrity)
- [SRI Hash Generator](https://www.srihash.org/)
- [W3C SRI Specification](https://www.w3.org/TR/SRI/)
- [cdnjs.com](https://cdnjs.com/) - CDN con SRI pre-calcolati
- [jsdelivr.com](https://www.jsdelivr.com/) - CDN con SRI pre-calcolati

---

## ?? Metriche Progetto

| Metrica | Valore |
|---------|--------|
| **Librerie CDN Totali** | 18 |
| **Librerie con SRI** | 13 (72%) |
| **Hash Corretti** | 13/13 (100%) |
| **Librerie Bloccate** | 0 |
| **File Documentazione** | 4 |
| **Script PowerShell** | 3 |
| **Tempo Fix Totale** | ~30 min |
| **Sicurezza Score** | ? Alta |

---

## ?? Risultati Raggiunti

- ? jQuery UI CSS: Hash corretto
- ? Axios: Hash corretto + versione fissa
- ? JSZip: Hash aggiunto
- ? Vue.js: Hash aggiunto
- ? Script automatici per verifica/update
- ? Documentazione completa
- ? Build stabile
- ? Pronto per deploy

---

## ?? Contatti

Per domande o supporto su questi fix:

1. Consulta documentazione in `/DOCS`
2. Esegui script di verifica
3. Controlla troubleshooting nei file MD
4. Se problema persiste, crea issue su GitHub

---

**Autore**: GitHub Copilot  
**Data**: 2025-01-24  
**Versione**: 1.0  
**Status**: ? Completato
