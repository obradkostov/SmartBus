import { API_URL, STORAGE_KEYS } from './config.js';

// ──────── Storage ────────────────────────────────────────────────
const Storage = {
	getAccessToken: () => localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN),
	getRefreshToken: () => localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN),
	setTokens: (accessToken, refreshToken) => {
		localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, accessToken);
		localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
	},
	clear: () => {
		localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
		localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
		localStorage.removeItem(STORAGE_KEYS.USER);
	}
};

function buildHeaders(token, extraHeaders = {}) {
	return {
		'Content-Type': 'application/json',
		'Accept': '*/*',
		...(token && { 'Authorization': `Bearer ${token}` }),
		...extraHeaders
	};
}

// ──────── Refresh ────────────────────────────────────────────────

async function tryRefreshToken() {
	const refreshToken = Storage.getRefreshToken();
	if(!refreshToken) return false;
	
	try {
		const res = await fetch(`${API_URL}/RefreshToken/Refresh`, {
			method: 'POST',
			headers: buildHeaders(null),
			body: JSON.stringify({ refreshToken })
		});
		
		if(!res.ok) return false;
		
		const { accessToken, refreshToken: newRefresh } = await res.json();
		Storage.setTokens(accessToken, newRefresh);
		return true;
	}catch{
		return false;
	}
}

function odjavi() {
    Storage.clear();
    window.location.href = './login.html';
}

// ──────── Fetch wrapper ───────────────────────────────────────────────

async function apiFetch(endpoint, options = {}) {
	const { public: isPublic = false, headers: extraHeaders = {}, ...fetchOptions } = options;
	
	const response = await fetch(`${API_URL}${endpoint}`, {
		...fetchOptions,
		headers: buildHeaders(Storage.getAccessToken(), extraHeaders)
	});
	
	if(response.status !== 401) return response;
	
	const refreshed = await tryRefreshToken();
	
	if(refreshed) return apiFetch(endpoint, options);
	
	if(!isPublic) odjavi();
	return null;
}

export const AutobusService = {
	getAll: () => apiFetch('/Autobus/VratiAutobuse'),
	add: (data) => apiFetch('/Autobus/DodajAutobus', {
		method: 'POST',
		body: JSON.stringify(data)
	}),
	remove: (id) => apiFetch(`/Autobus/ObrisiAutobus/${id}`, {
		method: 'DELETE',
	}),
	update: (data) => apiFetch('/Autobus/IzmeniAutobus', {
		method: 'PUT',
		body: JSON.stringify(data)
	})
}

export const KorisnikService = {
	getByEmail: (email) => apiFetch(`/Korisnik/VratiKorisnikaEmail/${email}`),
	getAllKontrolors: () => apiFetch('/Korisnik/VratiKontrolore'),
	add: (data) => apiFetch('/Korisnik/DodajKorisnika', {
		method: 'POST',
		body: JSON.stringify(data)
	}),
	remove: (id) => apiFetch(`/Korisnik/ObrisiKorisnika/${id}`, {
		method: 'DELETE',
	}),
	update: (data) => apiFetch('/Korisnik/IzmeniKorisnika', {
		method: 'PUT',
		body: JSON.stringify(data)
	}),
	changeName: (id, ime) => apiFetch(`/Korisnik/IzmeniKorisnickoIme/${id}/${ime}`, {
		method: 'PUT'
	}),
	logout: (refreshToken) => apiFetch('/Korisnik/Logout', {
		method: 'POST',
		body: JSON.stringify({ refreshToken })
	}),
	changePassword: (data) => apiFetch('/Korisnik/PromeniLozinku', {
		method: 'PUT',
		body: JSON.stringify(data)
	})
}

export const PolazakService = {
	getAll: () => apiFetch('/Polazak/VratiPolaske'),
	getById: (id) => apiFetch(`/Polazak/${id}`),
	add: (data) => apiFetch('/Polazak/DodajPolazak', {
		method: 'PUT',
		body: JSON.stringify(data)
	}),
	remove: (id) => apiFetch(`/Polazak/ObrisiPolazak/${id}`, {
		method: 'DELETE',
	}),
	update: (data) => apiFetch('/Polazak/IzmeniPolazak', {
		method: 'PUT',
		body: JSON.stringify(data)
	}),
	filter: (departure, arrival, date) => apiFetch(`/Polazak/Filtriraj/${departure}/${arrival}/${date}`, { public: true }),
}

export const ObavestenjaService = {
	getAll: () => apiFetch('/Obavestenje/VratiObavestenja'),
	add: (data) => apiFetch('/Obavestenje/DodajObavestenje', {
		method: 'POST',
		body: JSON.stringify(data)
	})
}

export const StanicaService = {
	getAll: () => apiFetch('/Stanica/VratiStanice', { public: true }),
}

export const KartaService = {
	getHistory: (id) => apiFetch(`/Karta/IstorijaKarata/${id}`),
	getOccupied: (id) => apiFetch(`/Karta/ZauzetaSedista/${id}`),
	add: (data) => apiFetch('/Karta/DodajKartu', {
		method: 'POST',
		body: JSON.stringify(data)
	}),
	signature: (id) => apiFetch(`/Karta/Signature/${id}`),
	validate: (data) => apiFetch('/Karta/ValidirajKartu', {
		method: 'POST',
		body: JSON.stringify(data)
	})
}

export const OcenaAutobusService = {
	add: (data) => apiFetch('/OcenaAutobus/DodajOcenu', {
		method: 'POST',
		body: JSON.stringify(data)
	})
}

export const SedisteService = {
	getAll: (id) => apiFetch(`/Sediste/VratiSedista/${id}`)
}

export const IzvestajService = {
	add: (data) => apiFetch('/DnevniIzvestaj/DodajDnevniIzvestaj', {
		method: 'POST',
		body: JSON.stringify(data)
	}),
	getByFilter: (data) => apiFetch('/DnevniIzvestaj/VratiIzvestajePaginacija', {
		method: "POST",
		body: JSON.stringify(data)
	}),
	getStats: () => apiFetch('/DnevniIzvestaj/VratiStatistiku')
}