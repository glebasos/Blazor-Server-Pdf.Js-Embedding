// wwwroot/js/pdf-restrictions.js

let keydownHandler = null;
let iframeKeydownHandler = null;
let injectedStyle = null;
let mutationObserver = null;

export function initPdfRestrictions() {
    // Block Ctrl+P, Ctrl+S, Ctrl+O on the main page
    keydownHandler = function (e) {
        if ((e.ctrlKey || e.metaKey) && ['p', 's', 'o'].includes(e.key.toLowerCase())) {
            e.preventDefault();
            e.stopPropagation();
            console.log('Print/Save/Open blocked on main page');
            return false;
        }
    };

    document.addEventListener('keydown', keydownHandler, true);

    // Wait for iframe to load and modify it
    const iframe = document.getElementById('pdfViewerIframe');

    if (iframe) {
        iframe.addEventListener('load', function () {
            try {
                const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
                const iframeWindow = iframe.contentWindow;

                // Hide print/download/open buttons once toolbar is ready
                hidePdfButtonsWhenReady(iframeDoc);

                // Block keyboard shortcuts inside iframe
                iframeKeydownHandler = function (e) {
                    if ((e.ctrlKey || e.metaKey) && ['p', 's', 'o'].includes(e.key.toLowerCase())) {
                        e.preventDefault();
                        e.stopPropagation();
                        console.log('Print/Save/Open blocked in iframe');
                        return false;
                    }
                };
                iframeDoc.addEventListener('keydown', iframeKeydownHandler, true);

                // Override print function
                if (iframeWindow) {
                    iframeWindow.print = function () {
                        console.log('Print function blocked');
                        return false;
                    };
                }

                // Add custom CSS to hide buttons
                addCustomStyles(iframeDoc);

            } catch (e) {
                console.error('Cannot access iframe content:', e);
                console.log('This is normal if PDF.js is loaded from a different origin');
            }
        });
    }
}

function hidePdfButtonsWhenReady(iframeDoc) {
    const buttonsToHide = [
        'printButton',
        'downloadButton',
        'secondaryPrint',
        'secondaryDownload',
        'openFile',
        'secondaryOpenFile'
    ];

    const tryHide = () => {
        buttonsToHide.forEach(id => {
            const el = iframeDoc.getElementById(id);
            if (el) {
                el.style.display = 'none';
            }
        });
    };

    // Try immediately
    tryHide();

    // Observe DOM for dynamically inserted buttons
    mutationObserver = new MutationObserver(() => tryHide());
    mutationObserver.observe(iframeDoc.body, { childList: true, subtree: true });

    // Stop after 3s
    setTimeout(() => {
        if (mutationObserver) {
            mutationObserver.disconnect();
            mutationObserver = null;
        }
    }, 3000);
}

function addCustomStyles(iframeDoc) {
    injectedStyle = iframeDoc.createElement('style');
    injectedStyle.textContent = `
        #printButton,
        #downloadButton,
        #secondaryPrint,
        #secondaryDownload,
        #openFile,
        #secondaryOpenFile {
            display: none !important;
        }
    `;
    iframeDoc.head.appendChild(injectedStyle);
}

export function cleanup() {
    if (keydownHandler) {
        document.removeEventListener('keydown', keydownHandler, true);
        keydownHandler = null;
    }

    const iframe = document.getElementById('pdfViewerIframe');
    if (iframe) {
        try {
            const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;

            if (iframeKeydownHandler) {
                iframeDoc.removeEventListener('keydown', iframeKeydownHandler, true);
                iframeKeydownHandler = null;
            }

            if (injectedStyle) {
                injectedStyle.remove();
                injectedStyle = null;
            }

            if (mutationObserver) {
                mutationObserver.disconnect();
                mutationObserver = null;
            }
        } catch (e) {
            // Ignore if can't access iframe
        }
    }
}
