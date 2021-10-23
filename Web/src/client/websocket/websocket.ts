import { DebugRequest } from '../../api/websocket/common/DebugRequest';
import { PongResponse } from '../../api/websocket/common/PongResponse';
import { MessageBase } from '../../api/websocket/MessageBase';

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
      let session: WebSocketSession;
      websocket.onopen = (event) => {
        session = new WebSocketSession(websocket, url);
        this.onOpen(event);
        resolve(session);
      };
      websocket.onerror = (event) => {
        this.onError(event);
        if (!session) {
          reject(event);
        }
      };
      websocket.onclose = (event) => this.onClose(event as CloseEvent);
      websocket.onmessage = (event: MessageEvent) => {
        const data: MessageBase = JSON.parse(event.data);
        if (!this.onMessage(session, data)) {
          onMessage(data);
        }
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
  private onMessage(session: WebSocketSession, data: MessageBase): boolean {
    if (data.Type === 'Debug') {
      console.debug((data as DebugRequest).Data);
      return true;
    } else if (data.Type === 'Ping') {
      session.sendMessage({
        Type: 'Pong',
        Timestamp: new Date().getTime(),
      });
      return true;
    } else if (data.Type === 'Pong') {
      console.debug(`Delay is ${new Date().getTime() - (data as PongResponse).Timestamp}`);
      return true;
    }
    return false;
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
