import { API_URL, STORAGE_KEYS } from "./config.js";

const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);

if (user) {
    if(refreshToken) {
        window.location.href = './dashboard.html';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('form');

    if (!form) {
        return;
    }

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        const email = form.email.value.trim();
        const password = form.password.value.trim();

        if (!email || !password) {
            alert('Unesite email i lozinku.');
            return;
        }

        const payload = {
            Email: email,
            Lozinka: password
        };

        try {
            const response = await fetch(`${API_URL}/Korisnik/Login`,
                {
                    method: 'POST',
                    headers: {
                        'Accept': '*/*',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                }
            );

            if (response.ok) {
                const user = await response.json();

                localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, user.accessToken);
                localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, user.refreshToken);
                localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify({
                    id: user.korisnikId,
                    email: user.email,
                    ime: user.ime,
                    uloga: user.uloga,
                }));
                window.location.href = './dashboard.html';
            } else {
                const error = await response.text();
                alert('Greška pri prijavi: ' + error);
            }
        } catch (error) {
            alert('Greška pri povezivanju sa serverom: ' + error.message);
        }
    });
});