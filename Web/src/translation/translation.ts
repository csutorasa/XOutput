import * as hungarian from './Hungarian.json';
import * as english from './English.json';

export class TranslationService {
    private data: { [key: string]: string };

    constructor() {
        this.setLanguage('English');
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
