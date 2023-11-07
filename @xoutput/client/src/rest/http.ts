export class HttpService {
  get<T>(path: string): Promise<T> {
    return fetch(`/api${path}`, {
      method: 'GET',
    }).then((r) => this.readBody(r, `/api${path}`));
  }

  post<T>(path: string, body?: object): Promise<T> {
    return fetch(`/api${path}`, {
      method: 'POST',
      body: body != null ? JSON.stringify(body) : null,
      headers: {
        'Content-Type': 'application/json',
      },
    }).then((r) => this.readBody(r, `/api${path}`));
  }

  put<T>(path: string, body?: object): Promise<T> {
    return fetch(`/api${path}`, {
      method: 'PUT',
      body: body != null ? JSON.stringify(body) : null,
      headers: {
        'Content-Type': 'application/json',
      },
    }).then((r) => this.readBody(r, `/api${path}`));
  }

  delete<T>(path: string, body?: object): Promise<T> {
    return fetch(`/api${path}`, {
      method: 'DELETE',
      body: body != null ? JSON.stringify(body) : null,
      headers: {
        'Content-Type': 'application/json',
      },
    }).then((r) => this.readBody(r, `/api${path}`));
  }

  private readBody<T>(r: Response, path: string): Promise<T> {
    return r.text().then((body) => {
      if (body) {
        try {
          return JSON.parse(body);
        } catch {
          throw new Error(
            `Failed to parse JSON from response body from ${path}. Response body: ` +
              (body.length > 1000 ? body.substring(0, 1000) + '...' : body)
          );
        }
      }
      return null;
    });
  }
}

export const http = new HttpService();
