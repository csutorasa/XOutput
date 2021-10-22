import { MessageBase } from './message';

export class WebSocketService {
  private static globalHost: string;
  private static globalPort: string | number;
  private host: string;
  private port: string | number;

  static initialize(host: string, port: string | number): void {
    this.globalHost = host;
    this.globalPort = port;
  }

  connect(path: string, onMessage: (data: MessageBase) => void): Promise<WebSocketSession> {
    this.host = WebSocketService.globalHost;
    this.port = WebSocketService.globalPort;
    return new Promise((resolve, reject) => {
      const url = `ws://${this.host}:${this.port}/ws/${path}`;
      const websocket = new WebSocket(url);
      websocket.onopen = (event) => {
        resolve(new WebSocketSession(websocket, url));
        this.onOpen(event);
      };
      websocket.onerror = (event) => this.onError(event);
      websocket.onclose = (event) => this.onClose(event as CloseEvent);
      websocket.onmessage = (event: MessageEvent) => {
        this.onMessage(event);
        const data = JSON.parse(event.data);
        onMessage(data);
      };
    });
  }
  private onOpen(event: Event): void {
    console.info('Connected to ' + this.host + ':' + this.port);
  }
  private onError(event: Event): void {
    const message: string = (event as any).message;
    console.error(message);
  }
  private onClose(event: CloseEvent): void {
    console.info('Disconnected from ' + this.host + ':' + this.port);
  }
  private onMessage(event: MessageEvent): void {
    //
  }
}

export class WebSocketSession {
  constructor(private websocket: WebSocket, private url: string) {}
  close(): void {
    this.websocket.close();
  }
  isReady(): boolean {
    return this.websocket && this.websocket.readyState === WebSocket.OPEN;
  }
  sendMessage(obj: { Type: string; [key: string]: any }): void {
    this.websocket.send(JSON.stringify(obj));
  }
  sendDebug(text: string): void {
    this.sendMessage({
      Type: 'Debug',
      Data: text,
    });
  }
}
