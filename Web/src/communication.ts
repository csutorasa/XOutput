export class Communication {
    private websocket: WebSocket;
    private host: string;
    private port: string | number;

    connect(host: string, port: string | number): void {
        this.websocket = new WebSocket("ws://" + host + ":" + port + "/");
        this.host = host;
        this.port = port;
        this.websocket.onopen = (event) => this.onOpen(event);
        this.websocket.onerror = (event) => this.onError(event);
        this.websocket.onclose = (event) => this.onClose(event as CloseEvent);
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
    isReady(): boolean {
        return this.websocket && this.websocket.readyState == WebSocket.OPEN;
    }
    private sendMessage(obj: object): void {
        this.websocket.send(JSON.stringify(obj));
    }
    private sendInputData(...data: { InputType: string, Value: number|boolean }[]): void {
        this.sendMessage({
            Type: "InputData",
            Data: data
        });
    }
    sendInput(input: string, value: number): void {
        this.sendInputData({
            InputType: input,
            Value: value,
        });
    }
    sendInputs(input1: string, value1: number, input2: string, value2: number): void {
        this.sendInputData({
            InputType: input1,
            Value: value1,
        }, {
            InputType: input2,
            Value: value2,
        });
    }
    sendDPad(up: number, down: number, left: number, right: number): void {
        this.sendInputData({
            InputType: "UP",
            Value: up,
        }, {
            InputType: "DOWN",
            Value: down,
        }, {
            InputType: "LEFT",
            Value: left,
        }, {
            InputType: "RIGHT",
            Value: right,
        });
    }
    sendDebug(text: string): void {
        this.sendMessage({
            Type: "Debug",
            Data: text
        });
    }
}
