import { Helper } from "./helper.js";
import QRCode from 'https://cdn.jsdelivr.net/npm/qrcode@1.5.3/+esm';
import jsQR from 'https://cdn.jsdelivr.net/npm/jsqr@1.4.0/+esm';

import { API_URL, STORAGE_KEYS } from "./config.js";
import { KartaService, KorisnikService, ObavestenjaService, PolazakService, StanicaService } from "./apiService.js";

document.addEventListener('DOMContentLoaded', async () => {
    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
    const profileCard = document.querySelector(".profile-card");
    if (!user) {
        profileCard.hidden = true;
    }
    else {
        const userName = document.querySelector('.user-name');
        const userEmail = document.querySelector('.user-email');
        const userRole = document.querySelector('.user-role');
        const userTickets = document.querySelector('.user-tickets');

        if (userName) userName.textContent = user.ime || 'Korisniče';
        if (userEmail) userEmail.textContent = user.email || 'example@mail.com';
        if (userRole) userRole.textContent = user.uloga || 'Putnik';
        if (userTickets) userTickets.textContent = user.tickets != null ? user.tickets : '0'
    }

    const applyFilters = document.getElementById('apply-filters');
    // Funkcionalnost za zvezde
    const stars = document.querySelectorAll('.search-panel .fa-star');
    let selectedRating = 0;

    stars.forEach((star, index) => {
        star.addEventListener('mouseover', () => {
            updateStars(index + 1);
        });
        star.addEventListener('mouseout', () => {
            updateStars(selectedRating);
        });
        star.addEventListener('click', () => {
            selectedRating = index + 1;
            updateStars(selectedRating);
        });
    });

    function updateStars(rating) {
        stars.forEach((star, index) => {
            if (index < rating) {
                star.classList.remove('fa-regular');
                star.classList.add('fa-solid');
                star.style.color = '#ffd700';
            } else {
                star.classList.remove('fa-solid');
                star.classList.add('fa-regular');
                star.style.color = '#2a3f5d';
            }
        });
    }

    const selectorDeparture = document.getElementById('filter-departure');
    const selectorArrival = document.getElementById('filter-arrival');

    try {
        const res = await StanicaService.getAll();
        if (!res.ok) throw new Error('Greska pri ucitavanju stanica.');
        const stanice = await res.json();

        for (var s of stanice) {
            const o1 = Helper.DodajElement('option', [], selectorDeparture, s.naziv);
            o1.value = s.naziv;
            const o2 = Helper.DodajElement('option', [], selectorArrival, s.naziv);
            o2.value = s.naziv;
        }
    }
    catch (err) {
        console.log(err);
    }

    if (selectorDeparture) {
        $(selectorDeparture).select2({
            placeholder: "Izaberite grad polaska",
            allowClear: false,
            width: '100%'
        });
    }

    if (selectorArrival) {
        $(selectorArrival).select2({
            placeholder: "Izaberite grad dolaska",
            allowClear: false,
            width: '100%'
        });
    }

    if (applyFilters) {
        applyFilters.onclick = async () => {
            const departure = document.getElementById('filter-departure').value;
            const arrival = document.getElementById('filter-arrival').value;
            const date = document.getElementById('filter-date').value;

            try {
                const response = await PolazakService.filter(departure, arrival, date);
                if (!response.ok) throw new Error('Greska pri filtriranju.');
                let filtriraniPolasci = await response.json();
                if (selectedRating > 0) {
                    filtriraniPolasci = filtriraniPolasci.filter(p => {
                        const ocene = p.autobus?.oceneAutobusa;
                        if (!ocene || ocene.length === 0) return false;
                        const prosek = ocene.reduce((sum, o) => sum + o.ocena, 0) / ocene.length;
                        return prosek >= selectedRating;
                    });
                }
                prikaziPolaske(filtriraniPolasci);
            }
            catch (err) {
                console.log(err)
            }
        }
    }

    function prikaziPolaske(polasci) {
        console.log(polasci);
        const polasciContainer = document.getElementById('polasci-container');
        Helper.ObrisiElemente(polasciContainer);
        if (!polasci.length) {
            Helper.DodajElement('p', [], polasciContainer, 'Nema polazaka za zadane kriterijume.');
            return;
        }

        for (const p of polasci) {
            const kartica = Helper.DodajElement('article', ['polasci-card'], polasciContainer);
            Helper.DodajElement('h3', [], kartica, p.autobus.prevoznik);
            Helper.DodajElement('p', [], kartica, `Ruta: ${p.stanicaOd.naziv} - ${p.stanicaDo.naziv}`);
            Helper.DodajElement('p', [], kartica, `Polazak: ${new Date(p.vremePolaska).toLocaleString('sr-RS')}`);
            Helper.DodajElement('p', [], kartica, `Peron: ${p.peron}`);
            Helper.DodajElement('p', [], kartica, `Cena: ${p.cena} din.`);
            const ocene = p.autobus?.oceneAutobusa;
            const prosek = ocene && ocene.length ?
                (ocene.reduce((sum, o) => sum + o.ocena, 0) / ocene.length).toFixed(1) : 'Nema ocena';
            Helper.DodajElement('p', [], kartica, `Ocena: ${prosek}`);
            const btnPonuda = Helper.DodajElement('button', [], kartica, 'Pogledaj ponudu');
            btnPonuda.innerHTML = '<i class="fa-solid fa-eye"></i> Pogledaj ponudu';
            btnPonuda.onclick = () => {
                window.location.href = `./polazak.html?id=${p.id}`;
            }
        }
    }
    const swapBtn = document.getElementById('swap-btn');
    if (swapBtn) {
        swapBtn.addEventListener('click', () => {
            const departure = $('#filter-departure');
            const arrival = $('#filter-arrival');
            const tempValue = departure.val();
            departure.val(arrival.val()).trigger('change');
            arrival.val(tempValue).trigger('change');
        });
    }

    const btnOdjava = document.querySelector('.btn-odjava');
    if (btnOdjava) {
        btnOdjava.addEventListener('click', async () => {
            const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);

            if (refreshToken) {
                try {
                    await KorisnikService.logout(refreshToken);
                } catch (err) {
                    console.error("Greška pri odjavi:", err);
                }
            }

            localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
            localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
            localStorage.removeItem(STORAGE_KEYS.USER);
            window.location.href = './login.html';
        });
    }

    const bell = document.getElementById("bell");
    const panel = document.getElementById("notifications");

    if (bell) {
        bell.addEventListener("click", async () => {
            panel.classList.toggle("hidden");

            const notifDiv = bell.closest('.notification-div');
            const rect = notifDiv.getBoundingClientRect();
            if (rect.left < window.innerWidth / 2) {
                notifDiv.classList.add('otvori-desno');
            } else {
                notifDiv.classList.remove('otvori-desno');
            }

            if (!panel.classList.contains("hidden")) {
                try {
                    const res = await ObavestenjaService.getAll();
                    if (!res.ok) throw new Error("Greška pri učitavanju obaveštenja.");
                    const data = await res.json();

                    if (data.tip !== 'dnevni_izvestaj') {
                        panel.innerHTML = "";
                        data.forEach(o => {
                            const div = document.createElement("div");
                            div.classList.add("notification");
                            div.classList.add(o.tip);
                            div.innerHTML = `
    <strong>${o.naslov}</strong><br>
    ${o.poruka}<br>
    <small>${new Date(o.vremeKreiranja).toLocaleString('sr-RS')}</small>
  `;
                            panel.appendChild(div);
                        });
                    }
                } catch (err) {
                    console.error(err);
                }
            }
        });
    }
    if (user.uloga === "admin") {
        const btnAdminPanel = Helper.DodajElement('button', ['admin'], profileCard);
        btnAdminPanel.innerHTML = '<i class="fa-solid fa-gauge"></i> Admin Panel';
        btnAdminPanel.onclick = () => {
            window.location.href = './admin.html';
        };
        const btnIzvestaji = Helper.DodajElement('button', ['izvestaj'], profileCard);
        btnIzvestaji.innerHTML = '<i class="fa-regular fa-file-lines"></i> Dnevni izvestaji';
        btnIzvestaji.onclick = () => {
            window.location.href = './izvestajAdmin.html';
        };
    }
    
    if (user.uloga === "kontrolor") {
        const btnValidacija = Helper.DodajElement("button", [], profileCard, "Validacija karte");
        btnValidacija.innerHTML = '<i class="fa-sharp fa-solid fa-circle-check"></i> Validacija karte';
        // Container za skener i rezultat
        const skenerContainer = Helper.DodajElement("div", ["skener-container"], profileCard);
        skenerContainer.style.display = "none";

        const video = Helper.DodajElement("video", ["skener-video"], skenerContainer);
        video.autoplay = true;
        video.playsInline = true;
        video.muted = true;

        const resultBox = Helper.DodajElement("div", ["skener-result"], skenerContainer);
        resultBox.style.display = "none";

        const btnStop = Helper.DodajElement("button", ["btn-stop-skener"], skenerContainer, "Zatvori kameru");

        let stream = null;
        let animFrame = null;
        const canvas = document.createElement("canvas");
        const ctx = canvas.getContext("2d");

        function stopSkener() {
            if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
            if (animFrame) { cancelAnimationFrame(animFrame); animFrame = null; }
            skenerContainer.style.display = "none";
            btnValidacija.disabled = false;
            btnValidacija.textContent = "Validacija karte";
        }

        async function obradiQR(decodedText) {
            let ticketData;
            try {
                ticketData = JSON.parse(decodedText);
            } catch {
                resultBox.style.display = "block";
                resultBox.textContent = "Neispravan QR kod.";
                resultBox.className = "skener-result rezultat-greska";
                return;
            }

            if (!ticketData.ticket_id || !ticketData.signature) {
                resultBox.style.display = "block";
                resultBox.textContent = "QR kod nije karta.";
                resultBox.className = "skener-result rezultat-greska";
                return;
            }

            resultBox.style.display = "block";
            resultBox.textContent = "Proveravanje...";
            resultBox.className = "skener-result rezultat-info";

            const payload = {
                kartaId: ticketData.ticket_id,
                signature: ticketData.signature,
                skeniranoU: new Date().toISOString()
            }

            try {
                const res = await KartaService.validate(payload);

                const result = await res.json();

                if (result.validirana) {
                    alert("Uspesno validirana karta");
                    resultBox.className = "skener-result rezultat-uspeh";
                    resultBox.innerHTML = `
                    <strong>✓ Karta je validna</strong>
                    ${result.kartaInfo ? `
                        <p>${result.kartaInfo.putnikIme}</p>
                        <p>${result.kartaInfo.relacija}</p>
                        <p>Sedište: ${result.kartaInfo.brojSedista}</p>
                    ` : ""}
                `;
                } else {
                    alert(result.poruka);
                    resultBox.className = "skener-result rezultat-greska";
                    resultBox.innerHTML = `<strong>✗ ${result.poruka}</strong>`;
                }
            } catch {
                resultBox.className = "skener-result rezultat-greska";
                resultBox.textContent = "Greška pri komunikaciji sa serverom.";
            }
        }

        function skenirajFrame() {
            if (video.readyState === video.HAVE_ENOUGH_DATA) {
                canvas.width = video.videoWidth;
                canvas.height = video.videoHeight;
                ctx.drawImage(video, 0, 0);
                const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                const code = jsQR(imageData.data, imageData.width, imageData.height);
                if (code) {
                    stopSkener();
                    obradiQR(code.data);
                    return;
                }
            }
            animFrame = requestAnimationFrame(skenirajFrame);
        }

        btnStop.onclick = stopSkener;

        btnValidacija.onclick = async () => {
            resultBox.style.display = "none";

            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                alert("Ovaj uređaj nema podršku za kameru.");
                return;
            }

            try {
                stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } });
                video.srcObject = stream;
                await video.play();
                skenerContainer.style.display = "block";
                btnValidacija.disabled = true;
                btnValidacija.textContent = "Skeniranje u toku...";
                animFrame = requestAnimationFrame(skenirajFrame);
            } catch (err) {
                if (err.name === "NotFoundError") {
                    alert("Kamera nije pronađena na ovom uređaju.");
                } else if (err.name === "NotAllowedError") {
                    alert("Pristup kameri je odbijen.");
                } else {
                    alert("Kamera nije dostupna: " + err.message);
                }
            }
        };

        const btnIzvestaj = Helper.DodajElement('button', [], profileCard, 'Dnevni Izvestaj');
        btnIzvestaj.innerHTML = '<i class="fa-solid fa-chart-bar"></i> Dnevni izveštaj';

        btnIzvestaj.onclick = () => {
            window.location.href = './izvestaj.html';
        }
    }
});
