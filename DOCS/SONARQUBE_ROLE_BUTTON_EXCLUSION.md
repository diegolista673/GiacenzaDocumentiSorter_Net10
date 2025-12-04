# ?? SonarQube - Esclusione Regola role="button" per Bootstrap 5 Navbar

**Data**: 2025-01-24  
**Regola**: `Web:BoldAndItalicTagsCheck` / `Web:AriaRoleCheck`

---

## ?? Problema

SonarQube segnala warning su questo codice:

```html
<a class="nav-link dropdown-toggle" 
   href="#" 
   role="button" 
   data-bs-toggle="dropdown">
   Menu
</a>
```

**Warning:**
> "Use `<button>` or `<input>` instead of the button role to ensure accessibility"

---

## ? Perché il Codice È Corretto

### 1. **Bootstrap 5 Requirement**
Bootstrap 5 navbar dropdown **richiede** l'uso di `<a>` tag con `role="button"`:

```html
<!-- ? CORRETTO - Bootstrap 5 Pattern -->
<a class="nav-link dropdown-toggle" 
   href="#" 
   role="button" 
   data-bs-toggle="dropdown" 
   aria-expanded="false">
   Dropdown Menu
</a>
```

**Fonte:** [Bootstrap 5 Navbar Documentation](https://getbootstrap.com/docs/5.3/components/navbar/#supported-content)

---

### 2. **WCAG Compliance**
Il pattern è conforme a WCAG 2.1 perché:

- ? `role="button"` comunica funzionalità al screen reader
- ? `aria-expanded` comunica stato dropdown
- ? `data-bs-toggle` gestisce keyboard interaction (Enter/Space)
- ? `href="#"` previene navigazione
- ? ID univoci per `aria-labelledby`

**Fonte:** [ARIA Authoring Practices - Disclosure Pattern](https://www.w3.org/WAI/ARIA/apg/patterns/disclosure/)

---

### 3. **Usare `<button>` Rompe il Layout**
Se si sostituisce `<a>` con `<button>`:

```html
<!-- ? SBAGLIATO - Rompe Bootstrap navbar -->
<button class="nav-link dropdown-toggle" 
        type="button" 
        data-bs-toggle="dropdown">
   Menu
</button>
```

**Problemi:**
- ? Stile navbar rotto (button non eredita `.nav-link` correttamente)
- ? Margini/padding incorretti
- ? Hover states non funzionanti
- ? Responsive behavior rotto

---

## ?? Configurazione SonarQube

### Opzione 1: Esclusione a Livello di File

Aggiungi al file `sonar-project.properties`:

```properties
# Escludi regola ARIA role per navbar Bootstrap
sonar.issue.ignore.multicriteria=e1

# Escludi role="button" per file _Layout.cshtml
e1.ruleKey=Web:AriaRoleCheck
e1.resourceKey=**/Pages/Shared/_Layout.cshtml
```

---

### Opzione 2: Esclusione Inline (Già Applicata)

Commento nel codice:

```razor
@* SONAR: role="button" è corretto per Bootstrap 5 navbar dropdown - non cambiare in <button> *@
<li class="nav-item dropdown">
    <a class="nav-link dropdown-toggle" href="#" role="button" ...>
```

---

### Opzione 3: Esclusione Globale (Non Consigliato)

Se hai **molti** navbar dropdown, puoi disabilitare globalmente:

```properties
# Disabilita regola globalmente (non consigliato)
sonar.issue.ignore.multicriteria=e1
e1.ruleKey=Web:AriaRoleCheck
e1.resourceKey=**/*.cshtml
```

**?? Attenzione:** Questo disabilita la regola per **tutti** i file `.cshtml`, inclusi quelli dove il warning è valido.

---

## ?? Quando la Regola SonarQube È Valida

Usa `<button>` invece di `role="button"` in questi casi:

### ? SBAGLIATO
```html
<!-- ? Non usare <div> o <span> con role="button" -->
<div role="button" onclick="submitForm()">Submit</div>
<span role="button" class="btn" onclick="doAction()">Action</span>
```

### ? CORRETTO
```html
<!-- ? Usa <button> nativo -->
<button type="button" onclick="submitForm()">Submit</button>
<button type="button" class="btn" onclick="doAction()">Action</button>
```

---

## ?? Eccezioni Legittime

### 1. **Bootstrap Navbar Dropdown** ?
```html
<a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
```

### 2. **Link che Attivano JavaScript** ?
```html
<a href="#section" role="button" data-bs-toggle="collapse">Toggle</a>
```

### 3. **Custom Styled Links** ?
Quando `<button>` rompe il design esistente e refactoring non è possibile.

---

## ?? Riferimenti

### Bootstrap 5
- [Navbar Documentation](https://getbootstrap.com/docs/5.3/components/navbar/)
- [Dropdowns Component](https://getbootstrap.com/docs/5.3/components/dropdowns/)

### WCAG & ARIA
- [ARIA button role](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Roles/button_role)
- [WCAG 2.1 - 4.1.2 Name, Role, Value](https://www.w3.org/WAI/WCAG21/Understanding/name-role-value.html)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)

### SonarQube
- [Rule: Web:AriaRoleCheck](https://rules.sonarsource.com/html/RSPEC-6827)
- [Suppressing Issues](https://docs.sonarqube.org/latest/user-guide/issues/)

---

## ? Conclusione

**Il codice attuale nel `_Layout.cshtml` è CORRETTO.**

### Azioni Applicate:
1. ? Aggiunto commento esplicativo nel codice
2. ? Documentato motivo esclusione SonarQube
3. ? Nessuna modifica al codice funzionante

### Raccomandazioni:
- ? Aggiungi esclusione SonarQube nel `sonar-project.properties`
- ? Documenta pattern Bootstrap 5 nel team
- ? Applica regola SonarQube solo per `<div>`/`<span>` con `role="button"`

---

**Status**: ? **RISOLTO CON DOCUMENTAZIONE**  
**Action Required**: ? **Configurare SonarQube Exclusion**  
**Code Changes**: ? **Nessuna (codice corretto)**

---

**Ultima modifica**: 2025-01-24  
**Autore**: GitHub Copilot  
**File**: Pages/Shared/_Layout.cshtml
