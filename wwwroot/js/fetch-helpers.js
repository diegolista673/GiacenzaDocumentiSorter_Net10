/**
 * Fetch API Helpers - Sostituzione jquery.unobtrusive-ajax.js
 * @version 1.0
 */

export const FetchHelpers = {
    
    /**
     * POST con FormData e aggiornamento DOM
     * @param {string} url - URL handler (es: "?handler=InsertScatola")
     * @param {FormData} formData - Dati form
     * @param {string} updateElementId - ID elemento da aggiornare
     * @param {Function} onSuccess - Callback successo (opzionale)
     * @param {Function} onError - Callback errore (opzionale)
     */
    async postFormWithUpdate(url, formData, updateElementId, onSuccess = null, onError = null) {
        try {
            // Ottieni antiforgery token
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                body: formData,
                credentials: 'same-origin'
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const html = await response.text();
            
            // Aggiorna DOM
            const target = document.getElementById(updateElementId);
            if (target) {
                target.innerHTML = html;
            }

            // Callback successo
            if (onSuccess) onSuccess(html, response);

            return html;

        } catch (error) {
            console.error('Fetch error:', error);
            
            if (onError) {
                onError(error);
            } else {
                alert('Si è verificato un errore. Riprova.');
            }
            
            throw error;
        }
    },

    /**
     * Reinizializza DataTable
     */
    reinitDataTable(tableId, options = {}) {
        const defaults = { scrollX: true, order: [], ...options };
        
        if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
            $(`#${tableId}`).DataTable().destroy();
        }
        
        $(`#${tableId}`).DataTable(defaults);
    },

    /**
     * Vai all'ultima pagina DataTable
     */
    dataTableGoToLastPage(tableId) {
        $(`#${tableId}`).DataTable().page('last').draw('page');
    },

    /**
     * Pulisci input
     */
    clearInputs(inputIds) {
        inputIds.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = '';
        });
    },

    /**
     * Mostra/nascondi spinner
     */
    showSpinner(elementId) {
        const el = document.getElementById(elementId);
        if (el) el.style.display = 'block';
    },

    hideSpinner(elementId) {
        const el = document.getElementById(elementId);
        if (el) el.style.display = 'none';
    }
};

export default FetchHelpers;