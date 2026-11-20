import { IzvestajService, KorisnikService } from "./apiService.js";
import { Helper } from "./helper.js"

document.addEventListener('DOMContentLoaded', async () => {

    const izvestajiDanas = document.getElementById("stat-izvestaji");
    const pregledaneKarte = document.getElementById("stat-karte");
    const prekrsajniNalozi = document.getElementById("stat-nalozi");
    const naplaceneKazne = document.getElementById("stat-kazne");

    try {
        const response = await IzvestajService.getStats();
        if (!response.ok) {
            const error = await response.text();
            console.error(error);
            return;
        }
        const stats = await response.json();

        izvestajiDanas.textContent = stats.izvestajiDanas;
        izvestajiDanas.classList.remove("loading");
        pregledaneKarte.textContent = stats.pregledaneKarte;
        pregledaneKarte.classList.remove("loading");
        prekrsajniNalozi.textContent = stats.prekrsajniNalozi;
        prekrsajniNalozi.classList.remove("loading");
        naplaceneKazne.textContent = stats.naplaceneKazne;
        naplaceneKazne.classList.remove("loading");
    } catch (e) {
        console.log(e);
    }

    const btnPrimeniFiltere = document.getElementById("btn-primeni-filtere");
    const btnResetujFiltere = document.getElementById("btn-resetuj-filtere");
    const filterDatum = document.getElementById("filter-datum");
    const filterKontrolor = document.getElementById("filter-kontrolor");
    const filterSmena = document.getElementById("filter-smena");
    const tableBody = document.getElementById("tabela-body");
    const tableFooter = document.querySelector(".table-footer");

    try {
        const response = await KorisnikService.getAllKontrolors();
        if (response.ok) {
            const kontrolori = await response.json();
            for(let k of kontrolori){
                const optionFilterKontrolor = Helper.DodajElement("option", [], filterKontrolor, k.email);
                optionFilterKontrolor.value = k.id;
            }
        } else {
            const error = await response.text();
            console.error(error);
        }
    } catch (e) {
        console.log(e);
    }

    const prvaStrana = Helper.DodajElement("div", ["first-page"], tableFooter, "1");
    const prethodnaStrana = Helper.DodajElement("div", ["page-back"], tableFooter);
    Helper.DodajElement("i", ["fa-solid", "fa-arrow-left"], prethodnaStrana);
    const trenutnaStrana = Helper.DodajElement("label", ["current-page"], tableFooter, "1");
    const sledecaStrana = Helper.DodajElement("div", ["page-next"], tableFooter);
    Helper.DodajElement("i", ["fa-solid", "fa-arrow-right"], sledecaStrana);
    const poslednjaStrana = Helper.DodajElement("div", ["last-page"], tableFooter, "1");

    const prikazPoStrani = Helper.DodajElement("input", ["page-size"], tableFooter);
    prikazPoStrani.type = "number";
    prikazPoStrani.value = 5;
    prikazPoStrani.min = 5;

    // Jedna funkcija za pravljenje filter objekta
    const napraviFilter = (strana) => ({
        strana,
        velicinaStrane: Number(prikazPoStrani.value),
        datum: filterDatum.value || null,
        kontrolorId: filterKontrolor.value ? Number(filterKontrolor.value) : null,
        smena: filterSmena.value || null
    });

    const ucitajIzvestaje = async (filter) => {
        try {
            const response = await IzvestajService.getByFilter(filter);
            if (!response.ok) {
                const error = await response.text();
                console.error(error);
                return;
            }
            const podaci = await response.json();

            trenutnaStrana.textContent = podaci.trenutnaStrana;
            poslednjaStrana.textContent = podaci.ukupnoStrana;

            Helper.ObrisiElemente(tableBody);
            for (let izvestaj of podaci.listaIzvestaja) {
                const row = Helper.DodajElement("tr", [], tableBody);
                const tdIme = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", ["cell-name"], tdIme, `${izvestaj.ime}`);
                Helper.DodajElement("div", ["cell-sub"], tdIme, izvestaj.email);
                const tdDatum = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdDatum, izvestaj.datum);
                const tdSmena = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdSmena, izvestaj.smena);
                const tdRelacije = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdRelacije, izvestaj.relacije);
                const tdKarte = Helper.DodajElement("td", [], row);
                Helper.DodajElement("span", ["karte-valid"], tdKarte, izvestaj.validneKarte);
                Helper.DodajElement("span", [], tdKarte, " / ");
                Helper.DodajElement("span", ["karte-invalid"], tdKarte, izvestaj.nevalidneKarte);
                const tdBezKarte = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdBezKarte, izvestaj.putniciBezKarte);
                const tdNalozi = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdNalozi, izvestaj.prekrsajniNalozi);
                const tdKazne = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdKazne, izvestaj.iznosPlacenihKazni);
                const tdIncidenti = Helper.DodajElement("td", [], row);
                Helper.DodajElement("div", [], tdIncidenti, izvestaj.verbalniIncidenti +
                    izvestaj.fizickiIncidenti);
            }
        } catch (e) {
            console.log(e);
        }
    }

    // Inicijalno ucitavanje
    await ucitajIzvestaje(napraviFilter(1));

    btnResetujFiltere.onclick = () => {
        filterDatum.value = '';
        filterKontrolor.value = '';
        filterSmena.value = '';
    }

    btnPrimeniFiltere.onclick = async () => {
        await ucitajIzvestaje(napraviFilter(1));
    }

    prvaStrana.onclick = async () => {
        await ucitajIzvestaje(napraviFilter(1));
    }

    prethodnaStrana.onclick = async () => {
        const strana = Number(trenutnaStrana.textContent);
        if (strana > 1) {
            await ucitajIzvestaje(napraviFilter(strana - 1));
        }
    }

    sledecaStrana.onclick = async () => {
        const strana = Number(trenutnaStrana.textContent);
        const ukupno = Number(poslednjaStrana.textContent);
        if (strana < ukupno) {
            await ucitajIzvestaje(napraviFilter(strana + 1));
        }
    }

    poslednjaStrana.onclick = async () => {
        const ukupno = Number(poslednjaStrana.textContent);
        await ucitajIzvestaje(napraviFilter(ukupno));
    }
})