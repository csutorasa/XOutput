import { DebugRequest, MessageBase, PongResponse } from '@xoutput/api';

export class WebSocketService {
  private host: string;
  private port: string | number;

  initialize(host: string, port: string | number): void {
    this.host = host;
    this.port = port;
  }

  connect(path: string, onMessage: (data: MessageBase) => void): Promise<WebSocketSession> {
    return new Promise((resolve, reject) => {
      const url = `ws://${this.host}:${this.port}/websocket/${path}`;
      const websocket = new WebSocket(url);
      let session: WebSocketSession;
      let pingInterval: NodeJS.Timeout;
      websocket.onopen = (event) => {
        session = new WebSocketSession(websocket);
        this.onOpen(event);
        pingInterval = setInterval(() => {
          session.sendMessage({
            type: 'Ping',
            timestamp: new Date().getTime(),
          });
        }, 5000);
        resolve(session);
      };
      websocket.onerror = (event) => {
        this.onError(event);
        if (!session) {
          reject(event);
        }
      };
      websocket.onclose = (event) => {
        this.onClose(pingInterval, event as CloseEvent);
        if (!session) {
          reject(event);
        }
      };
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
  private onClose(interval: NodeJS.Timeout, event: CloseEvent): void {
    console.info('Disconnected from ' + this.host + ':' + this.port);
    if (interval) {
      clearInterval(interval);
    }
  }
  private onMessage(session: WebSocketSession, data: MessageBase): boolean {
    if (data.type === 'Debug') {
      console.debug((data as DebugRequest).data);
      return true;
    } else if (data.type === 'Ping') {
      session.sendMessage({
        type: 'Pong',
        timestamp: new Date().getTime(),
      });
      return true;
    } else if (data.type === 'Pong') {
      console.debug(`Delay is ${new Date().getTime() - (data as PongResponse).timestamp} ms`);
      return true;
    }
    return false;
  }
}

export class WebSocketSession {
  constructor(private websocket: WebSocket) {}
  close(): void {
    this.websocket.close();
  }
  isReady(): boolean {
    return this.websocket && this.websocket.readyState === WebSocket.OPEN;
  }
  sendMessage<T extends MessageBase>(obj: T): void {
    this.websocket.send(JSON.stringify(obj));
  }
  sendDebug(text: string): void {
    this.sendMessage({
      type: 'Debug',
      data: text,
    });
  }
}

export const websocket = new WebSocketService();
