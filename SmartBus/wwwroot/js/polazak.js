import { Helper } from "./helper.js";
import { STORAGE_KEYS } from "./config.js";
import { KartaService, PolazakService, SedisteService } from "./apiService.js";

document.addEventListener('DOMContentLoaded', async () => {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
    if (!user) {
        window.location.href = './login.html';
        return;
    }

    const urlParams = new URLSearchParams(window.location.search);
    const polazakId = parseInt(urlParams.get('id'));

    if (!polazakId) {
        alert("Nije prosleđen ID polaska!");
        window.location.href = './dashboard.html';
        return;
    }

    let cenaPolaska = 0;

    try {
        const res = await PolazakService.getById(polazakId);
        const polazak = await res.json();
        cenaPolaska = polazak.cena;

        document.querySelector('.detaljiPolaska').innerHTML = `<div class="polazak-info-glavni">
        <h2>${polazak.autobus?.prevoznik || 'Prevoznik'}</h2> 
        <p class="relacija"><strong>Ruta:</strong> ${polazak.stanicaOd?.naziv} - ${polazak.stanicaDo?.naziv}</p>
        <div class="detalji">
            <p><strong>Polazak:</strong> ${new Date(polazak.vremePolaska).toLocaleString()}</p>
            <p><strong>Peron:</strong> ${polazak.peron || '1'}</p>
            <p class="cena"><strong>Cena:</strong> ${polazak.cena} din.</p>
        </div>
    </div>`;

        const zauzetaRes = await KartaService.getOccupied(polazakId);
        const zauzetaSedista = await zauzetaRes.json();
        console.log("Zauzeta sedista:", zauzetaSedista);
        console.log("Broj sedista:", polazak.autobus?.brojSedista);
        const brojSedista = polazak.autobus?.brojSedista || 50;

        const sedistaContainer = document.querySelector('.sedista-container');
        sedistaContainer.innerHTML = `
        <p class="sedista-naslov">Izaberite sedište</p>
        <p class="sedista-legenda">
            <span class="leg-slobodno"></span> Slobodno 
            <span class="leg-zauzeto"></span> Zauzeto 
            <span class="leg-izabrano"></span> Izabrano
        </p>`;

        let izabranoSediste = null;

        let sedista = null;
        const response = await SedisteService.getAll(polazak.autobus?.id);
        if (response.ok) {
            sedista = await response.json();
            console.log(sedista);
        }
        Helper.ObrisiElemente(sedistaContainer);

        for (let sediste of sedista) {
            const btn = Helper.DodajElement("button", [], sedistaContainer, sediste.broj);
            btn.className = sediste.status === "zauzeto" ? 'sediste zauzeto' : 'sediste slobodno';
            btn.disabbled = sediste.status === "zauzeto" ? "true" : "false";
            btn.onclick = () => {
                document.querySelectorAll('.sediste.izabrano')
                    .forEach(s => s.classList.replace('izabrano', 'slobodno'));
                btn.classList.replace('slobodno', 'izabrano');
                izabranoSediste = sediste.broj;
            };
            sedistaContainer.appendChild(btn);
        }

        document.getElementById('btnKupi').addEventListener('click', async (e) => {
            e.preventDefault();
            if (!izabranoSediste) {
                alert("Molimo izaberite sedište!");
                return;
            }
            await kupiKartu(polazakId, izabranoSediste, cenaPolaska);
        });

    } catch (err) {
        console.error("Greška:", err);
    }

});

async function kupiKartu(polazakId, sediste, cena) {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
    if (!user) {
        alert("Morate biti ulogovani da biste kupili kartu!");
        window.location.href = './login.html';
        return;
    }

    const trenutniKorisnikId = parseInt(user.id || user.Id || user.korisnikId || user.KorisnikId);
    if (isNaN(trenutniKorisnikId)) {
        alert("Greška: Sistem ne može da pročita ID korisnika!");
        return;
    }

    const novaKarta = {
        //KorisnikId: user.id,
        Email: user.email,
        PolazakId: polazakId,
        Sediste: sediste,
        PlacenaCena: cena,
        Status: "kupljena",
        VremeKupovine: new Date().toISOString()
    };
    console.log("Šaljem:", JSON.stringify(novaKarta));
    try {
        const response = await KartaService.add(novaKarta);

        if (response.ok) {
            alert("Karta je uspešno kupljena i sačuvana u bazi!");
            window.location.href = './karte.html';
        } else {
            const errText = await response.text();
            console.log("Status:", response.status);
            console.log("Greška:", errText);
            alert("Greška pri kupovini: " + errText);
        }
    } catch (err) {
        console.error("Greška:", err);
    }
}