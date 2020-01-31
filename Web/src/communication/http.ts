export class HttpService {
    private host: string;
    private port: string;

    initialize(host: string, port: string) {
        this.host = host;
        this.port = port;
    }

    get<T>(path: string): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api/${path}`, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => r.json());
    }

    post<T>(path: string, body: object): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api/${path}`, {
            method: 'POST',
            body: JSON.stringify(body),
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => r.json());
    }
}

export const http = new HttpService();
