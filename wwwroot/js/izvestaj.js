import { Helper } from './helper.js';
import { STORAGE_KEYS } from './config.js';
import { IzvestajService } from './apiService.js';

document.addEventListener('DOMContentLoaded', async () => {
    const user = localStorage.getItem(STORAGE_KEYS.USER);
    const korisnik = JSON.parse(user);

    const Ime = document.getElementById('ime');
    Ime.value = korisnik.ime;
    Ime.disabled = true;
    const Email = document.getElementById('email');
    Email.value = korisnik.email;
    Email.disabled = true;

    function danasnjiDatum() {
        const danas = new Date();
        const godina = danas.getFullYear();
        const mesec = String(danas.getMonth() + 1).padStart(2, '0');
        const dan = String(danas.getDate()).padStart(2, '0');
        return `${godina}-${mesec}-${dan}`;
    }

    const Datum = document.getElementById('datum');
    Datum.value = danasnjiDatum();
    Datum.disabled = true;

    const btnSubmit = document.getElementById("sacuvaj");
    btnSubmit.onclick = async () => {

        const Smena = document.getElementById('smena').value;
        const Relacije = document.getElementById('relacije').value;

        const PregledaneKarte = document.getElementById('pregledaneKarte').value;
        const ValidneKarte = document.getElementById('validneKarte').value;
        const NevalidneKarte = document.getElementById('nevalidneKarte').value;
        const PutniciBezKarte = document.getElementById('putniciBezKarte').value;

        const PrekrsajniNalozi = document.getElementById('prekrsajniNalozi').value;
        const IznosPlacenihKazni = document.getElementById('iznosPlacenihKazni').value;
        const OdbijenaPlacanja = document.getElementById('odbijenaPlacanja').value;
        const PrijavljenaLica = document.getElementById('prijavljenaLica').value;

        const VerbalniIncidenti = document.getElementById('verbalniIncidenti').value;
        const FizickiIncidenti = document.getElementById('fizickiIncidenti').value;
        const PoziviPolicije = document.getElementById('poziviPolicije').value;
        const OpisIncidenata = document.getElementById('opisIncidenata').value;

        const NepravilnostiNaVozilu = document.getElementById('nepravilnostiNaVozilu').value;
        const ProblemiSaSistemom = document.getElementById('problemiSaSistemom').value;

        const dnevniIzvestaj = {
            email: korisnik.email,
            smena: Smena,
            relacije: Relacije,
            pregledaneKarte: Number(PregledaneKarte),
            validneKarte: Number(ValidneKarte),
            nevalidneKarte: Number(NevalidneKarte),
            putniciBezKarte: Number(PutniciBezKarte),
            prekrsajniNalozi: Number(PrekrsajniNalozi),
            iznosPlacenihKazni: Number(IznosPlacenihKazni),
            odbijenaPlacanja: Number(OdbijenaPlacanja),
            prijavljenaLica: Number(PrijavljenaLica),
            verbalniIncidenti: Number(VerbalniIncidenti),
            fizickiIncidenti: Number(FizickiIncidenti),
            poziviPolicije: Number(PoziviPolicije),
            opisIncidenata: OpisIncidenata,
            nepravilnostiNaVozilu: NepravilnostiNaVozilu,
            problemiSaSistemom: ProblemiSaSistemom
        };

        const response = await IzvestajService.add(dnevniIzvestaj);
        if (response.ok) {
            alert("Uspesno je kreiran dnevni izvestaj.");
            window.location.href = './dashboard.html';
        }
        else {
            try {
                const data = await response.text();
                alert(`Greska: ${data}`);
            }
            catch (e) {
                console.log(e);
            }
        }
    }
})


