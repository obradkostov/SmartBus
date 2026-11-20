/*export class Profile {
    constructor() {

    }

    Profile() {*/
import { KorisnikService } from "./apiService.js";
import { API_URL, STORAGE_KEYS } from "./config.js";

const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
if (!user) {
    location.href = './login.html';
    alert("Niste prijavljeni");
}
document.querySelector('.name').textContent = user.ime || 'Korisnik';
document.querySelector('.email').textContent = user.email || '';
document.querySelector('.uloga').textContent = user.uloga || 'putnik';
document.getElementById('input-ime').value = user.ime || '';
document.getElementById('input-email').value = user.email || '';
//Prikaz lozinke
document.querySelectorAll('.toggle-pw').forEach(icon => {
    icon.addEventListener('click', () => {
        const input = icon.previousElementSibling;
        if (input && input.tagName == 'INPUT') {
            const isPw = input.type == 'password';
            input.type = isPw ? 'text' : 'password';
            icon.classList.toggle('fa-eye-slash', !isPw);
            icon.classList.toggle('fa-eye', isPw);
        }
    });
});
document.querySelectorAll('.btn-save')[0].addEventListener('click', async () => {
    const novoIme = document.getElementById('input-ime').value.trim();
    if (!novoIme) {
        alert('Ime ne može biti prazno.');
        return;
    }
    try {
        console.log(user);
        const res = await KorisnikService.changeName(user.id, novoIme);
        if (!res.ok) throw new Error(await res.text());
        user.ime = novoIme;
        localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
        document.querySelector('.name').textContent = novoIme;
        alert("Podaci su azurirani");
    }
    catch (e) {
        alert('Greška: ' + e.message);
    }
})
document.querySelectorAll('.btn-save')[1].addEventListener('click', async () => {
    const stara = document.getElementById('input-stara-lozinka').value;
    const nova = document.getElementById('input-nova-lozinka').value;
    const potvrda = document.getElementById('input-potvrda-lozinka').value;
    if (!stara || !nova || !potvrda) {
        alert('Popunite sva polja.');
        return;
    }
    console.log(user.email);
    if (nova !== potvrda) {
        alert('Nova lozinka i potvrda se ne poklapaju.');
        return;
    }
    const izmenjenKorisnik = {
        email: user.email,
        staraLozinka: stara,
        novaLozinka: nova
    }
    try {
        const res = await KorisnikService.changePassword(izmenjenKorisnik);
        if (!res.ok) throw new Error(await res.text());
        alert('Lozinka je promenjena.');
        document.getElementById('input-stara-lozinka').value = '';
        document.getElementById('input-nova-lozinka').value = '';
        document.getElementById('input-potvrda-lozinka').value = '';
    }
    catch (e) {
        alert('Greška: ' + e.message);
    }
});
/*   }
}

const profile = new Profile();
profile.Profile();*/