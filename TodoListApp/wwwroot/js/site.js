function initAutoResize() {
    const textareas = document.querySelectorAll('.auto-resize');

    textareas.forEach(textarea => {
        const updateHeight = () => {
            textarea.style.height = 'auto';
            textarea.style.height = `${textarea.scrollHeight}px`;

            const button = textarea.closest('form').querySelector('button');
            if (button) {
                button.style.height = `${textarea.offsetHeight + 48}px`;
            }
        };

        textarea.addEventListener('input', updateHeight);
        updateHeight();
    });
}

document.addEventListener('DOMContentLoaded', initAutoResize);