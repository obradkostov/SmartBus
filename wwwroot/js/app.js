import { API_URL, STORAGE_KEYS } from "./config.js";

const mainContainer = document.getElementsByClassName(["main-container"]);

document.addEventListener('DOMContentLoaded', () => {
    const buttonLogin = document.querySelector('.button-login');
    const buttonRegister = document.querySelector('.button-register');
    const buttonPregled = document.querySelector('.button-pregled');

    const user = JSON.parse(localStorage.getItem(STORAGE_KEYS.USER));
    const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    if (refreshToken && user) {
        buttonLogin.onclick = () => {
            window.location.href = './dashboard.html';
        }

        buttonRegister.onclick = () => {
            window.location.href = './dashboard.html';
        }
    } else {
        buttonLogin.onclick = () => {
            window.location.href = './login.html';
        }

        buttonRegister.onclick = () => {
            window.location.href = './signup.html';
        }
    }

    buttonPregled.onclick = () => {
        window.location.href = './dashboard.html';
    }
});