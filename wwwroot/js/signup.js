import { API_URL } from "./config.js";

const form = document.getElementById('form');
const messageElement = document.getElementById('message');

const showMessage = (text, success = true) => {
    if (!messageElement) return;
    messageElement.textContent = text;
    messageElement.style.color = success ? '#0f9d58' : '#d93025';
};

console.log(form);
if (form) {
    form.addEventListener('submit', async event => {
        event.preventDefault();

        const formData = new FormData(form);
        const firstName = formData.get('ime')?.toString().trim() ?? '';
        const lastName = formData.get('prezime')?.toString().trim() ?? '';
        const password = formData.get('password')?.toString().trim() ?? '';
        const confirm = formData.get('confirm')?.toString().trim() ?? '';
        const email = formData.get('email')?.toString().trim() ?? '';

        
        if (!firstName || !email || !password || !confirm) {
            showMessage('Popunite sva obavezna polja.', false);
            return;
        }

        if (password !== confirm) {
            showMessage('Lozinka i potvrda lozinke se ne poklapaju.', false);
            return;
        }

        const payload = {
            Ime: [firstName, lastName].filter(Boolean).join(' '),
            Email: email,
            Lozinka: password
        };

        try {
            const response = await fetch(`${API_URL}/Korisnik/Register`, {
                method: 'POST',
                headers: {
                    'Accept': '*/*',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            const text = await response.text();
            if (response.ok) {
                showMessage('Registracija uspešna! Možete se prijaviti.');
                form.reset();
                window.location.href = './login.html';
            } else {
                showMessage(text || 'Greška pri registraciji.', false);
            }
        } catch (error) {
            showMessage('Greška pri komunikaciji sa serverom.', false);
            console.error(error);
        }
    });
}
