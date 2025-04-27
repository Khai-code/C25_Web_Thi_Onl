function validateCode() {
    const code = document.getElementById('examCode').value;
    const errorElement = document.getElementById('errorMessage');

    if (!/^\d{8}$/.test(code)) {
        errorElement.style.display = 'block';
        return false;
    }
} 