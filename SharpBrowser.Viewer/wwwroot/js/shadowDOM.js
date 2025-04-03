if (!customElements.get("shadow-root-viewer")) {
    customElements.define("shadow-root-viewer", class extends HTMLElement {
        constructor() {
            super();
            this._shadow = this.attachShadow({ mode: "open" });
        }

        setContent(htmlContent, styleContent, componentRef, aClickHandlerMethodName) {
            const cleanedHtmlContent = htmlContent.replace(/<\/?body>/gi, '');
            const updatedStyleContent = styleContent.replace(/\bbody\b/gi, '.shadow-content');
            this._shadow.innerHTML = `
                <style>
                    :host {
                        display: block;
                        overflow: hidden; 
                        position: relative;
                        height: 100%;
                    }
                    ${updatedStyleContent}
                    .shadow-content {
                        position: relative;
                        height: 100%; 
                    }
                </style>
                <div class="shadow-content">
                    ${cleanedHtmlContent}
                </div>
            `;

            // Prevent fixed topbar
            const fixedElements = Array.from(this._shadow.querySelectorAll('*')).filter(el => {
                return getComputedStyle(el).position === 'fixed';
            });
            fixedElements.forEach(el => {
                el.style.position = 'absolute';
            });

            // Prevent clicks on <a> tags
            this._shadow.querySelectorAll('a').forEach(anchor => {
                anchor.addEventListener('click', event => {
                    event.preventDefault();
                    componentRef.invokeMethodAsync(aClickHandlerMethodName, anchor.href);
                });
            });
        }

        connectedCallback() {
            // TODO: set custom PF default content
        }
    });
}

export function setShadowContent(elementRef, htmlContent, styleContent, componentRef, aClickHandlerMethodName) {
    if (elementRef?.setContent) {
        elementRef.setContent(htmlContent, styleContent, componentRef, aClickHandlerMethodName);
    }
}