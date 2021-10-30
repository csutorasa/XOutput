export declare class HttpService {
    private host;
    private port;
    initialize(host: string, port: string): void;
    get<T>(path: string): Promise<T>;
    post<T>(path: string, body?: object): Promise<T>;
    put<T>(path: string, body?: object): Promise<T>;
    delete<T>(path: string, body?: object): Promise<T>;
    private readBody;
}
export declare const http: HttpService;
