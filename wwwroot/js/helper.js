    export class Helper {
    constructor() {

    }
    static DodajElement(tag, classes = [], parent, text = undefined) {
        if (!parent) {
            return;
        }
        const element = document.createElement(tag);
        for (let c of classes) {
            element.classList.add(c);
        }
        if (text !== undefined) {
            element.textContent = text;
        }
        parent.appendChild(element);
        return element;
    }
    static ObrisiElemente(parent) {
        if (!parent) return;
        while (parent.firstChild) {
            parent.removeChild(parent.firstChild)
        }
    }
    static async StatusNotifikacija(response, text = undefined){
        switch(response.status){
            case 200: {
                const dialog = this.DodajElement("dialog", ["notify-200"], document.body, text);
                dialog.show();
                setTimeout(() => dialog.close(), 3000);
                break;
            }
            case 404: {
                const dialog = this.DodajElement("dialog", ["notify-200"], document.body, "Popunite sva polja");
                dialog.show();
                setTimeout(() => dialog.close(), 3000);
                break;
            }
            default: {
                const errorText = await response.text();
                const dialog = this.DodajElement("dialog", ["notify-default"], document.body, errorText);
                dialog.show();
                setTimeout(() => dialog.close(), 3000);
                break;
            }
        }
    }
}