export class HttpService {
    private host: string;
    private port: string;

    initialize(host: string, port: string) {
        this.host = host;
        this.port = port;
    }

    get<T>(path: string): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api${path}`, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => this.readBody(r));
    }

    post<T>(path: string, body?: object): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api${path}`, {
            method: 'POST',
            body: body != null ? JSON.stringify(body) : null,
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => this.readBody(r));
    }

    put<T>(path: string, body?: object): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api${path}`, {
            method: 'PUT',
            body: body != null ? JSON.stringify(body) : null,
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => this.readBody(r));
    }

    delete<T>(path: string, body?: object): Promise<T> {
        return fetch(`http://${this.host}:${this.port}/api${path}`, {
            method: 'DELETE',
            body: body != null ? JSON.stringify(body) : null,
            headers: {
              'Content-Type': 'application/json'
            }
        }).then(r => this.readBody(r));
    }

    private readBody<T>(r: Response): Promise<T> {
        return r.text().then((body) => {
            if (body) {
                try {
                    return JSON.parse(body);
                } catch {
                    throw new Error('Failed to parse JSON from response body');
                }
            }
            return null;
        });
    }
}

export const http = new HttpService();
export default http;
