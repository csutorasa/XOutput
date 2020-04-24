import * as hungarian from './Hungarian.json';
import * as english from './English.json';

class TranslationService {
    private data: { [key: string]: string };

    constructor() {
        switch(navigator.language.slice(0, 2)) {
            case "hu":
                this.setLanguage('Hungarian');
                break;
            default:
                this.setLanguage('English');
                break;
        }
    }

    setLanguage(language: string) {
        if (language === 'Hungarian') {
            this.data = hungarian.default;
        } else {
            this.data = english.default;
        }
    }

    translate(key: string): string {
        return this.data[key] || key;
    }
}

export const Translation = new TranslationService();
export default Translation;
