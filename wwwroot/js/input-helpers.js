/**
 * Input Helpers - Utility functions for input handling
 * Replaces inline event handlers with cleaner event listeners
 */

export const InputHelpers = {
    
    /**
     * Initialize all input helpers on page load
     */
    init() {
        this.initUppercaseInputs();
    },
    
    /**
     * Setup uppercase transformation for inputs with data-transform="uppercase"
     * Usage: <input data-transform="uppercase" />
     */
    initUppercaseInputs() {
        document.querySelectorAll('[data-transform="uppercase"]').forEach(input => {
            input.addEventListener('input', (e) => {
                const start = e.target.selectionStart;
                const end = e.target.selectionEnd;
                
                e.target.value = e.target.value.toUpperCase();
                
                // Preserve cursor position
                e.target.setSelectionRange(start, end);
            });
        });
    },
    
    /**
     * Add uppercase transformation to specific input
     * @param {string|HTMLElement} input - Input element or selector
     */
    makeUppercase(input) {
        const element = typeof input === 'string' 
            ? document.querySelector(input) 
            : input;
            
        if (element) {
            element.addEventListener('input', (e) => {
                const start = e.target.selectionStart;
                const end = e.target.selectionEnd;
                
                e.target.value = e.target.value.toUpperCase();
                e.target.setSelectionRange(start, end);
            });
        }
    },
    
    /**
     * Validate required fields in a form
     * @param {HTMLFormElement} form - Form element
     * @returns {boolean} True if all required fields are valid
     */
    validateRequiredFields(form) {
        const requiredFields = form.querySelectorAll('[required]');
        let isValid = true;
        
        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                field.classList.add('is-invalid');
                isValid = false;
            } else {
                field.classList.remove('is-invalid');
            }
        });
        
        return isValid;
    },
    
    /**
     * Clear specific form inputs
     * @param {string[]} inputNames - Array of input names to clear
     */
    clearInputs(inputNames) {
        inputNames.forEach(name => {
            const input = document.querySelector(`[name="${name}"]`);
            if (input) {
                input.value = '';
            }
        });
    },
    
    /**
     * Focus on first input in container
     * @param {string|HTMLElement} container - Container element or selector
     */
    focusFirstInput(container) {
        const element = typeof container === 'string' 
            ? document.querySelector(container) 
            : container;
            
        if (element) {
            const firstInput = element.querySelector('input, select, textarea');
            if (firstInput) {
                firstInput.focus();
            }
        }
    },
    
    /**
     * Disable/enable all inputs in a container
     * @param {string|HTMLElement} container - Container element or selector
     * @param {boolean} disabled - True to disable, false to enable
     */
    toggleInputs(container, disabled) {
        const element = typeof container === 'string' 
            ? document.querySelector(container) 
            : container;
            
        if (element) {
            element.querySelectorAll('input, select, textarea, button').forEach(input => {
                input.disabled = disabled;
            });
        }
    }
};

// Auto-initialize on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => InputHelpers.init());
} else {
    InputHelpers.init();
}
