import { MessageBase } from "./message";

export class WebSocketService {
    private websocket: WebSocket;
    private static globalHost: string;
    private static globalPort: string | number;
    private host: string;
    private port: string | number;

    static initialize(host: string, port: string | number): void {
        this.globalHost = host;
        this.globalPort = port;
    }

    connect(path: string, onMessage: (data: MessageBase) => void): Promise<void> {
        this.host = WebSocketService.globalHost;
        this.port = WebSocketService.globalPort;
        return new Promise((resolve, reject) => {
            this.websocket = new WebSocket(`ws://${this.host}:${this.port}/ws/${path}`);
            this.websocket.onopen = (event) => {
                resolve();
                this.onOpen(event);
            };
            this.websocket.onerror = (event) => this.onError(event);
            this.websocket.onclose = (event) => this.onClose(event as CloseEvent);
            this.websocket.onmessage = (event: MessageEvent) => {
                this.onMessage(event);
                const data = JSON.parse(event.data);
                onMessage(data);
            };
        });
    }
    private onOpen(event: Event): void {
        console.log("Connected to " + this.host + ":" + this.port);
    }
    private onError(event: Event): void {
        const message: string = (<any>event).message;
        console.error(message);
        if (this.isReady()) {
            this.sendDebug(message);
        }
    }
    private onClose(event: CloseEvent): void {
        console.log("Disconnected from " + this.host + ":" + this.port);
        this.websocket = null;
    }
    private onMessage(event: MessageEvent): void {
        
    }
    close(): void {
        this.websocket.close();
    }
    isReady(): boolean {
        return this.websocket && this.websocket.readyState == WebSocket.OPEN;
    }
    sendMessage(obj: { Type: string; [key: string]: any }): void {
        this.websocket.send(JSON.stringify(obj));
    }
    sendDebug(text: string): void {
        this.sendMessage({
            Type: "Debug",
            Data: text
        });
    }
}
