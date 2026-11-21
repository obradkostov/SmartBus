import { Helper } from "./helper.js";
import { API_URL, STORAGE_KEYS } from "./config.js";
import { AutobusService, KorisnikService, ObavestenjaService, PolazakService, StanicaService } from "./apiService.js";

export class Admin {
    constructor() {
    }

    async Crtaj() {

        const [autobusiResponse, staniceResponse, polasciResponse] = await Promise.all([
            AutobusService.getAll(),
            StanicaService.getAll(),
            PolazakService.getAll()
        ]);

        const [autobusi, stanice, polasci] = await Promise.all([
            autobusiResponse.json(),
            staniceResponse.json(),
            polasciResponse.json()
        ])

        const mainContainer = document.querySelector(".main-container");

        Helper.DodajElement("h1", ["main-title"], mainContainer, "Admin panel");
        const container = Helper.DodajElement("div", ["crud-container"], mainContainer);


        const detailsAutobus = Helper.DodajElement("details", ["crud-category"], container);
        const summaryAutobus = Helper.DodajElement("summary", ["summary-category"], detailsAutobus, "Autobus");
        const iconA = document.createElement("i");
        iconA.className = "fas fa-chevron-down";
        summaryAutobus.appendChild(iconA);
        const groupAutobus = Helper.DodajElement("div", ["crud-group"], detailsAutobus);

        const detailsKorisnik = Helper.DodajElement("details", ["crud-category"], container);
        const summaryKorisnik = Helper.DodajElement("summary", ["summary-category"], detailsKorisnik, "Korisnik");
        const iconK = document.createElement("i");
        iconK.className = "fas fa-chevron-down";
        summaryKorisnik.appendChild(iconK);
        const groupKorisnik = Helper.DodajElement("div", ["crud-group"], detailsKorisnik);

        const detailsPolazak = Helper.DodajElement("details", ["crud-category"], container);
        const summaryPolazak = Helper.DodajElement("summary", ["summary-category"], detailsPolazak, "Polazak");
        const iconP = document.createElement("i");
        iconP.className = "fas fa-chevron-down";
        summaryPolazak.appendChild(iconP);
        const groupPolazak = Helper.DodajElement("div", ["crud-group"], detailsPolazak);

        // const detailsObavestenje = Helper.DodajElement("details", ["crud-category"], container);
        // const summaryObavestenje = Helper.DodajElement("summary", ["summary-category"], detailsObavestenje, "Obavestenje");
        // const iconO = document.createElement("i");
        // iconO.className = "fas fa-chevron-down";
        // summaryObavestenje.appendChild(iconO);
        // const groupObavestenje = Helper.DodajElement("div", ["crud-group"], detailsObavestenje);
        // --------------------------------Autobus--------------------------------
        // Dodavanje autobusa
        {
            const dodajAutobus = Helper.DodajElement("details", [], groupAutobus)
            const summary = Helper.DodajElement("summary", ["summary-card"], dodajAutobus, "Dodaj autobus");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], dodajAutobus);

            Helper.DodajElement("label", [], form, "Model: ");
            const inputModel = Helper.DodajElement("input", [], form);

            Helper.DodajElement("label", [], form, "Broj sedista: ");
            const inputBrojSedista = Helper.DodajElement("input", [], form);
            inputBrojSedista.type = "number";

            Helper.DodajElement("label", [], form, "Prevoznik: ");
            const inputPrevoznik = Helper.DodajElement("input", [], form);

            const dodajAutobusButton = Helper.DodajElement("button", ["full-row"], form, "Dodaj autobus");
            dodajAutobusButton.onclick = async () => {
                const noviAutobus = {
                    model: inputModel.value,
                    brojSedista: inputBrojSedista.value,
                    prevoznik: inputPrevoznik.value
                }
                const status = await AutobusService.add(noviAutobus);
                await Helper.StatusNotifikacija(status, "Uspesno dodat autobus.");
            }
        }
        // Brisanje autobusa
        {
            const obrisiAutobus = Helper.DodajElement("details", [], groupAutobus)
            const summary = Helper.DodajElement("summary", ["summary-card"], obrisiAutobus, "Obrisi autobus");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], obrisiAutobus);

            Helper.DodajElement("label", [], form, "Odaberi autobus: ");
            const autobusSelect = Helper.DodajElement("select", [], form);
            $(autobusSelect).select2();

            autobusi.forEach(autobus => {
                const option = Helper.DodajElement("option", [], autobusSelect, `${autobus.model}, ${autobus.brojSedista}, ${autobus.prevoznik}`);
                option.value = autobus.id;
            });

            const obrisiAutobusButton = Helper.DodajElement("button", ["full-row"], form, "Obrisi autobus");
            obrisiAutobusButton.onclick = async () => {
                const status = await AutobusService.remove(autobusSelect.value);
                await Helper.StatusNotifikacija(status, "Uspesno obrisan autobus.");
            }
        }
        // Izmena autobusa
        {
            const izmeniAutobus = Helper.DodajElement("details", [], groupAutobus)
            const summary = Helper.DodajElement("summary", ["summary-card"], izmeniAutobus, "Izmeni autobus");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], izmeniAutobus);

            Helper.DodajElement("label", [], form, "Odaberi autobus: ");
            const autobusSelect = Helper.DodajElement("select", [], form);
            $(autobusSelect).select2();

            autobusi.forEach(autobus => {
                const option = Helper.DodajElement("option", [], autobusSelect, `${autobus.model}, ${autobus.brojSedista}, ${autobus.prevoznik}`);
                option.value = autobus.id;
            });

            Helper.DodajElement("label", [], form, "Model: ");
            const inputModel = Helper.DodajElement("input", [], form);

            Helper.DodajElement("label", [], form, "Broj sedista: ");
            const inputBrojSedista = Helper.DodajElement("input", [], form);
            inputBrojSedista.type = "number";

            Helper.DodajElement("label", [], form, "Prevoznik: ");
            const inputPrevoznik = Helper.DodajElement("input", [], form);


            const izmeniAutobusButton = Helper.DodajElement("button", ["full-row"], form, "Izmeni autobus");
            izmeniAutobusButton.onclick = async () => {
                const izmenjenAutobus = {
                    id: autobusSelect.value,
                    model: inputModel.value,
                    brojSedista: inputBrojSedista.value,
                    prevoznik: inputPrevoznik.value
                }
                const status = await AutobusService.update(izmenjenAutobus);
                await Helper.StatusNotifikacija(status, "Uspesno izmenjen autobus.");
            }
        }
        // --------------------------------Korisnik--------------------------------
        // Dodavanje korisnika
        {
            const dodajKorisnika = Helper.DodajElement("details", [], groupKorisnik);
            const summary = Helper.DodajElement("summary", ["summary-card"], dodajKorisnika, "Dodaj korisnika");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], dodajKorisnika);

            Helper.DodajElement("label", [], form, "Korisnicko ime: ");
            const inputKorisnickoIme = Helper.DodajElement("input", [], form);

            Helper.DodajElement("label", [], form, "Email: ");
            const inputEmail = Helper.DodajElement("input", [], form);

            Helper.DodajElement("label", [], form, "Lozinka: ");
            const inputLozinka = Helper.DodajElement("input", [], form);

            Helper.DodajElement("label", [], form, "Uloga: ");
            const selectUloga = Helper.DodajElement("select", [], form);
            const uloge = ["admin", "kontrolor", "putnik"];

            uloge.forEach(uloga => {
                const option = Helper.DodajElement("option", [], selectUloga, uloga);
                option.value = uloga;
            });

            const dodajKorisnikaButton = Helper.DodajElement("button", ["full-row"], form, "Dodaj korisnika");
            dodajKorisnikaButton.onclick = async () => {
                const noviKorisnik = {
                    ime: inputKorisnickoIme.value,
                    email: inputEmail.value,
                    lozinka: inputLozinka.value,
                    uloga: selectUloga.value,
                };
                console.log(noviKorisnik);
                const status = await KorisnikService.add(noviKorisnik);
                await Helper.StatusNotifikacija(status, "Uspesno dodat korisnik.");
            };
        }
        // Brisanje korisnika
        {
            const obrisiKorisnika = Helper.DodajElement("details", [], groupKorisnik);
            const summary = Helper.DodajElement("summary", ["summary-card"], obrisiKorisnika, "Obrisi korisnika");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], obrisiKorisnika);

            Helper.DodajElement("label", [], form, "Email: ");
            const inputEmail = Helper.DodajElement("input", [], form);

            const buttonNadji = Helper.DodajElement("button", ["full-row-button"], form, "Nadji korisnika");
            const subForm = Helper.DodajElement("div", ["sub-form"], form);


            buttonNadji.onclick = async () => {
                const response = await KorisnikService.getByEmail(inputEmail.value);
                if (!response.ok) {
                    await Helper.StatusNotifikacija(response);
                }
                const korisnik = await response.json();
                Helper.ObrisiElemente(subForm);

                Helper.DodajElement("label", [], subForm, "Korisnicko ime:");
                Helper.DodajElement("label", [], subForm, korisnik.ime);

                Helper.DodajElement("label", [], subForm, "Email:");
                Helper.DodajElement("label", [], subForm, korisnik.email);

                Helper.DodajElement("label", [], subForm, "Uloga:");
                Helper.DodajElement("label", [], subForm, korisnik.uloga);

                const buttonObrisi = Helper.DodajElement("button", ["full-row"], subForm, "Obrisi korisnika");
                buttonObrisi.onclick = async () => {
                    const status = await KorisnikService.remove(korisnik.id);
                    await Helper.StatusNotifikacija(status, "Uspesno obrisan korisnik.");
                }
            }
        }
        // Izmena korisnika
        {
            const izmeniKorisnika = Helper.DodajElement("details", [], groupKorisnik);
            const summary = Helper.DodajElement("summary", ["summary-card"], izmeniKorisnika, "Izmeni korisnika");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], izmeniKorisnika);

            Helper.DodajElement("label", [], form, "Email: ");
            const inputEmail = Helper.DodajElement("input", [], form);

            const buttonNadji = Helper.DodajElement("button", ["full-row-button"], form, "Nadji korisnika");

            const subForm = Helper.DodajElement("div", ["sub-form"], form);
            buttonNadji.onclick = async () => {
                const response = await KorisnikService.getByEmail(inputEmail.value);
                if (response.ok) {
                    const korisnik = await response.json();
                    console.log(korisnik);
                    Helper.ObrisiElemente(subForm);

                    Helper.DodajElement("label", [], subForm, "Korisnicko ime: ");
                    const inputKorisnickoIme = Helper.DodajElement("input", [], subForm);
                    inputKorisnickoIme.value = korisnik.ime;

                    Helper.DodajElement("label", [], subForm, "Email: ");
                    const inputEmail = Helper.DodajElement("input", [], subForm);
                    inputEmail.value = korisnik.email;

                    Helper.DodajElement("label", [], subForm, "Lozinka: ");
                    const inputLozinka = Helper.DodajElement("input", [], subForm);
                    inputLozinka.value = korisnik.lozinka;

                    Helper.DodajElement("label", [], subForm, "Uloga: ");
                    const selectUloga = Helper.DodajElement("select", [], subForm);
                    const uloge = ["admin", "kontrolor", "putnik"];
                    uloge.forEach(uloga => {
                        const option = Helper.DodajElement("option", [], selectUloga, uloga);
                        option.value = uloga;
                    });
                    selectUloga.value = korisnik.uloga;

                    const izmeniKorisnikaButton = Helper.DodajElement("button", ["full-row"], subForm, "Izmeni korisnika");
                    izmeniKorisnikaButton.onclick = async () => {
                        const izmenjenKorisnik = {
                            id: korisnik.id,
                            ime: inputKorisnickoIme.value,
                            email: inputEmail.value,
                            lozinka: inputLozinka.value,
                            uloga: selectUloga.value
                        };
                        const response2 = await KorisnikService.update(izmenjenKorisnik);
                        await Helper.StatusNotifikacija(response2, "Uspesno izmenjen korisnik.");
                    };
                }
                else {
                    Helper.StatusNotifikacija(response);
                }
            };
        }
        // --------------------------------Polazak--------------------------------
        // Dodavanje polaska
        {
            const dodajPolazak = Helper.DodajElement("details", [], groupPolazak);
            const summary = Helper.DodajElement("summary", ["summary-card"], dodajPolazak, "Dodaj polazak");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], dodajPolazak);

            Helper.DodajElement("label", [], form, "Stanica polaska: ");
            const selectStanicaPolaska = Helper.DodajElement("select", [], form);
            $(selectStanicaPolaska).select2({
                tags: true,
                placeholder: "Izaberite ili upišite novi grad",
                allowClear: true
            });
            stanice.forEach(stanica => {
                const option = Helper.DodajElement("option", [], selectStanicaPolaska, `${stanica.naziv}`);
                option.value = stanica.id;
            });

            Helper.DodajElement("label", [], form, "Stanica dolaska: ");
            const selectStanicaDolaska = Helper.DodajElement("select", [], form);
            $(selectStanicaDolaska).select2({
                tags: true,
                placeholder: "Izaberite ili upišite novi grad",
                allowClear: true
            });
            stanice.forEach(stanica => {
                const option = Helper.DodajElement("option", [], selectStanicaDolaska, `${stanica.naziv}`);
                option.value = stanica.id;
            });

            Helper.DodajElement("label", [], form, "Cena: ");
            const inputCena = Helper.DodajElement("input", [], form);
            inputCena.type = "number";

            Helper.DodajElement("label", [], form, "Peron: ");
            const inputPeron = Helper.DodajElement("input", [], form);
            inputPeron.type = "number";

            Helper.DodajElement("label", [], form, "Datum i vreme: ");
            const inputDatumVreme = Helper.DodajElement("input", [], form);
            inputDatumVreme.type = "datetime-local";

            Helper.DodajElement("label", [], form, "Autobus: ");
            const autobusSelect = Helper.DodajElement("select", [], form);
            $(autobusSelect).select2();
            autobusi.forEach(autobus => {
                const option = Helper.DodajElement("option", [], autobusSelect, `${autobus.model}, ${autobus.brojSedista}, ${autobus.prevoznik}`);
                option.value = autobus.id;
            });

            const buttonDodajPolazak = Helper.DodajElement("button", ["full-row"], form, "Dodaj");
            buttonDodajPolazak.onclick = async () => {
                const noviPolazak = {
                    stanicaOdId: selectStanicaPolaska.value,
                    stanicaDoId: selectStanicaDolaska.value,
                    cena: inputCena.value,
                    peron: inputPeron.value,
                    vremePolaska: inputDatumVreme.value,
                    autobusId: autobusSelect.value
                };
                const status = await PolazakService.add(noviPolazak);
                Helper.StatusNotifikacija(status, "Uspesno dodat polazak.");
            }
        }
        // Brisanje polaska
        {
            const obrisiPolazak = Helper.DodajElement("details", [], groupPolazak);
            const summary = Helper.DodajElement("summary", ["summary-card"], obrisiPolazak, "Obrisi polazak");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], obrisiPolazak);

            Helper.DodajElement("label", [], form, "Odaberi polazak: ");
            const polazakSelect = Helper.DodajElement("select", [], form);
            $(polazakSelect).select2();

            polasci.forEach(polazak => {
                const option = Helper.DodajElement("option", [], polazakSelect, `Stanica polaska: ${polazak.stanicaOd.naziv}, Stanica dolaska: ${polazak.stanicaDo.naziv}, Datum i vreme: ${new Date(polazak.vremePolaska).toLocaleString()}`);
                option.value = polazak.id;
            });

            const obrisiPolazakButton = Helper.DodajElement("button", ["full-row"], form, "Obrisi polazak");
            obrisiPolazakButton.onclick = async () => {
                const status = await PolazakService.remove(polazakSelect.value);
                Helper.StatusNotifikacija(status, "Uspesno obrisan polazak.");
            }
        }
        // Izmena polaska
        {
            const izmeniPolazak = Helper.DodajElement("details", [], groupPolazak);
            const summary = Helper.DodajElement("summary", ["summary-card"], izmeniPolazak, "Izmeni polazak");

            const icon = document.createElement("i");
            icon.className = "fas fa-chevron-down";
            summary.appendChild(icon);

            const form = Helper.DodajElement("div", ["form-grid"], izmeniPolazak);

            Helper.DodajElement("label", [], form, "Izmeni polazak: ");
            const polazakSelect = Helper.DodajElement("select", [], form);
            $(polazakSelect).select2();
            polasci.forEach(polazak => {
                const option = Helper.DodajElement("option", [], polazakSelect, `${polazak.stanicaOd.naziv}, ${polazak.stanicaDo.naziv}, ${new Date(polazak.vremePolaska).toLocaleString()}`);
                option.value = polazak.id;
            });

            const subForm = Helper.DodajElement("div", ["sub-form"], form);
            polazakSelect.onchange = () => {
                const polazak = polasci.find(p => p.id == polazakSelect.value);

                Helper.ObrisiElemente(subForm);

                Helper.DodajElement("label", [], subForm, "Stanica polaska:");
                const selectStanicaPolaska = Helper.DodajElement("select", ["select2-container"], subForm);
                $(selectStanicaPolaska).select2();
                stanice.forEach(stanica => {
                    const option = Helper.DodajElement("option", [], selectStanicaPolaska, `${stanica.naziv}`);
                    option.value = stanica.id;
                });
                selectStanicaPolaska.value = polazak.stanicaOd.id;

                Helper.DodajElement("label", [], subForm, "Stanica dolaska:");
                const selectStanicaDolaska = Helper.DodajElement("select", [], subForm);
                $(selectStanicaDolaska).select2();
                stanice.forEach(stanica => {
                    const option = Helper.DodajElement("option", [], selectStanicaDolaska, `${stanica.naziv}`);
                    option.value = stanica.id;
                });
                selectStanicaDolaska.value = polazak.stanicaDo.id;

                Helper.DodajElement("label", [], subForm, "Cena:");
                const inputCena = Helper.DodajElement("input", [], subForm);
                inputCena.type = "number";
                inputCena.value = polazak.cena;

                Helper.DodajElement("label", [], subForm, "Peron:");
                const inputPeron = Helper.DodajElement("input", [], subForm);
                inputPeron.type = "number";
                inputPeron.value = polazak.peron;

                Helper.DodajElement("label", [], subForm, "Vreme polaska:");
                const inputDatumVreme = Helper.DodajElement("input", [], subForm);
                inputDatumVreme.type = "datetime-local";
                inputDatumVreme.value = new Date(polazak.vremePolaska).toISOString().slice(0, 16);

                Helper.DodajElement("label", [], subForm, "Autobus:");
                const autobusSelect = Helper.DodajElement("select", [], subForm);
                $(autobusSelect).select2();
                autobusi.forEach(autobus => {
                    const option = Helper.DodajElement("option", [], autobusSelect, `${autobus.model}, ${autobus.brojSedista}, ${autobus.prevoznik}`);
                    option.value = autobus.id;
                });
                autobusSelect.value = polazak.autobus.id;

                const buttonIzmeni = Helper.DodajElement("button", ["full-row"], subForm, "Izmeni polazak");
                buttonIzmeni.onclick = async () => {
                    //pozvati apiService za izmenu polaska
                }

                Helper.DodajElement("label", [], subForm, "Naslov:");
                const inputNaslov = Helper.DodajElement("input", [], subForm);
                Helper.DodajElement("label", ["top"], subForm, "Poruka:");
                const textAreaPoruka = Helper.DodajElement("textarea", [], subForm);
                textAreaPoruka.rows = "10";

                Helper.DodajElement("label", [], subForm, "Tip: ");
                const selectTip = Helper.DodajElement("select", [], subForm);
                const tipovi = ["izmena", "vanredna_situacija"];
                const nazivi = ["Izmena", "Vanredna situacija"];

                for (let t = 0; t < tipovi.length; t++) {
                    const option = Helper.DodajElement("option", [], selectTip, nazivi[t]);
                    option.value = tipovi[t];
                }

                const dodajObavestenjeButton = Helper.DodajElement("button", ["full-row"], subForm, "Dodaj obavestenje");
                dodajObavestenjeButton.onclick = async () => {
                    const novoObavestenje = {
                        polazakId: polazak.id,
                        naslov: inputNaslov.value,
                        poruka: textAreaPoruka.value,
                        tip: selectTip.value
                    };
                    const response = await ObavestenjaService.add(novoObavestenje);
                    Helper.StatusNotifikacija(response, "Uspesno dodato obavestenje");
                }
            }
        }

        // --------------------------------Obavestenje--------------------------------
        //Dodavanje obavestenja
        // {
        //     const dodajObavestenje = Helper.DodajElement("details", [], groupObavestenje);
        //     const summary = Helper.DodajElement("summary", ["summary-card"], dodajObavestenje, "Dodaj obavestenje");

        //     const icon = document.createElement("i");
        //     icon.className = "fas fa-chevron-down";
        //     summary.appendChild(icon);

        //     const form = Helper.DodajElement("div", ["form-grid"], dodajObavestenje);

        //     Helper.DodajElement("label", [], form, "Polazak:");
        //     const selectPolazak = Helper.DodajElement("select2", [], form);

        //     Helper.DodajElement("label", [], form, "Naslov:");
        //     const inputNaslov = Helper.DodajElement("input", [], form);
        //     Helper.DodajElement("label", ["top"], form, "Opis:");
        //     const textAreaOpis = Helper.DodajElement("textarea", [], form);
        //     textAreaOpis.rows = "10";

        //     Helper.DodajElement("label", [], form, "Tip: ");
        //     const selectTip = Helper.DodajElement("select", [], form);
        //     const tipovi = ["izmena", "vanredna_situacija"];
        //     const nazivi = ["Izmena", "Vanredna situacija"];

        //     for (let t = 0; t < tipovi.length; t++) {
        //         const option = Helper.DodajElement("option", [], selectTip, nazivi[t]);
        //         option.value = tipovi[t];
        //     }

        //     const dodajObavestenjeButton = Helper.DodajElement("button", ["full-row"], form, "Dodaj obavestenje");
        //     dodajObavestenjeButton.onclick = async () => {
        //         const novoObavestenje = {
                    //     polazakId: polazak.id,
                    //     naslov: inputNaslov.value,
                    //     poruka: textAreaPoruka.value,
                    //     tip: selectTip.value
                    // };
                    // const response = await ObavestenjaService.add(novoObavestenje);
                    // Helper.StatusNotifikacija(response, "Uspesno dodato obavestenje");
        //     }

        // }
        // Stanica
    }
}

const admin = new Admin();
await admin.Crtaj();