import { Helper } from "./helper.js";
import QRCode from 'https://cdn.jsdelivr.net/npm/qrcode@1.5.3/+esm';
import { API_URL, STORAGE_KEYS } from "./config.js";
import { KartaService, OcenaAutobusService } from "./apiService.js";

document.addEventListener('DOMContentLoaded', async () => {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
    if (!user) {
        alert("Morate biti prijavljeni!");
        window.location.href = './login.html';
        return;
    }
    const container = document.getElementById('karte-container');
    try {
        const res = await KartaService.getHistory(user.id);
        if (!res.ok) throw new Error(await res.text());
        const karte = await res.json();
        if (!karte.length) {
            Helper.DodajElement('p', ['nema-karata'], container, 'Nemate kupljenih karata.');
            return;
        }
        for (const k of karte) {
            const kartica = Helper.DodajElement('div', ['karta-kartica'], container);

            const status = k.status === 'kupljena' ? 'status-kupljena'
                : k.status === 'validirana' ? 'status-validirana'
                    : 'status-otkazana';
            Helper.DodajElement('span', ['status-badge', status], kartica, k.status.toUpperCase());

            Helper.DodajElement('h3', ['karta-prevoznik'], kartica, k.prevoznik || '—');
            Helper.DodajElement('p', ['podaci'], kartica, `Ruta: ${k.stanicaOd} - ${k.stanicaDo}`);
            Helper.DodajElement('p', ['podaci'], kartica, `Polazak: ${new Date(k.vremePolaska).toLocaleString('sr-RS')}`);
            Helper.DodajElement('p', ['podaci'], kartica, `Sedište: ${k.sediste}`);
            Helper.DodajElement('p', ['podaci'], kartica, `Cena: ${k.cena} din.`);

            if (k.status !== 'otkazana') {
                try {
                    const sigRes = await KartaService.signature(k.id);
                    if (sigRes.ok) {
                        const { signature } = await sigRes.json();

                        const qrData = JSON.stringify({
                            ticket_id: k.id,
                            korisnik_id: user.id,
                            polazak_id: k.polazakId,
                            signature: signature
                        });

                        const qrCanvas = Helper.DodajElement('canvas', ['karta-qr'], kartica);
                        await QRCode.toCanvas(qrCanvas, qrData, { width: 180, margin: 1 });
                    }
                } catch (err) {
                    console.error(`Greška pri generisanju QR za kartu ${k.id}:`, err);
                }

                const btnOceni = Helper.DodajElement('button', ['btn-oceni'], kartica, 'Oceni Prevoznika');
                btnOceni.innerHTML = '<i class="fa-solid fa-star"></i> Oceni prevoznika';
                btnOceni.onclick = () => Oceni(k, kartica, btnOceni);

                const btnOtkazi = Helper.DodajElement('button', ['btn-otkazi'], kartica, 'Otkazi kartu');
                btnOtkazi.innerHTML = '<i class="fa-solid fa-circle-xmark"></i> Otkaži kartu';
                btnOtkazi.onclick = async () => {
                    if (!confirm("Da li ste sigurni da želite da otkažete kartu?")) return;
                    const token = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
                    console.log("Stvarni token koji šaljem na bekend:", token);

                    try {
                        const res = await fetch(`${API_URL}/Karta/OtkaziKartu/${k.id}`, {
                            method: 'PUT',
                            headers: {
                                'Content-Type': 'application/json',
                                'Authorization': `Bearer ${token}`
                            }
                        });
                        if (res.ok) {
                            kartica.remove();
                            alert("Karta je uspešno otkazana.");
                        } else {
                            const porukaGreske = await res.text();
                            alert("Greška: " + (porukaGreske || res.statusText));
                        }
                    } catch (err) {
                        console.error(err);
                        alert("Došlo je do greške prilikom mrežnog zahteva.");
                    }
                };
            }
        }
    }
    catch (err) {
        console.error("Greška:", err);
        Helper.DodajElement('p', ['greska'], container, 'Greška pri učitavanju karata.');
    }
    function Oceni(karta, kartica, btn) {
        btn.remove();
        const ocenaDiv = Helper.DodajElement('div', ['ocena-forma'], kartica);
        Helper.DodajElement('p', ['ocena-label'], ocenaDiv, 'Ocenite prevoznika');
        const zvezdiceDiv = Helper.DodajElement('div', ['ocena-zvezdice'], ocenaDiv);
        let izabranaOcena = 0;
        for (let i = 1; i <= 5; i++) {
            const star = Helper.DodajElement('span', ['zvezda'], zvezdiceDiv, '★');
            star.dataset.value = i;
            star.onmouseover = () => osvetliZvezdice(zvezdiceDiv, i);
            star.onmouseout = () => osvetliZvezdice(zvezdiceDiv, izabranaOcena);
            star.onclick = () => { izabranaOcena = i; osvetliZvezdice(zvezdiceDiv, i); };
        }
        const btnPosalji = Helper.DodajElement('button', ['btn-posalji-ocenu'], ocenaDiv, 'Pošalji ocenu');
        btnPosalji.innerHTML = '<i class="fa-solid fa-paper-plane"></i> Pošalji ocenu';
        btnPosalji.onclick = async () => {
            if (!izabranaOcena) { alert("Izaberite ocenu!"); return; }
            const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
            const novaOcena = {
                AutobusId: karta.autobusId,
                KorisnikId: user.id,
                Ocena: izabranaOcena
            }
            console.log();
            const res = await OcenaAutobusService.add(novaOcena);
            if (!res.ok) {
                alert("Greška pri ocenjivanju: " + await res.text());
            } else {
                alert("Autobus uspesno ocenjen.");
            }
        };
        function osvetliZvezdice(container, broj) {
            container.querySelectorAll('.zvezda').forEach((s, i) => {
                s.style.color = i < broj ? '#f59e0b' : '#d1d5db';
            });
        }
    }
});