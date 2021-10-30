import { MessageBase } from '@xoutput/api';
export declare class WebSocketService {
    private host;
    private port;
    initialize(host: string, port: string | number): void;
    connect(path: string, onMessage: (data: MessageBase) => void): Promise<WebSocketSession>;
    private onOpen;
    private onError;
    private onClose;
    private onMessage;
}
export declare class WebSocketSession {
    private websocket;
    private url;
    constructor(websocket: WebSocket, url: string);
    close(): void;
    isReady(): boolean;
    sendMessage<T extends MessageBase>(obj: T): void;
    sendDebug(text: string): void;
}
export declare const websocket: WebSocketService;
