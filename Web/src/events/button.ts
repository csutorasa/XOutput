import { WebSocketService } from "../communication/websocket";
import { AbstractInputFlow, UIInputEvent } from "./base";

export class ButtonFlow extends AbstractInputFlow<number> {
    private key: string;
    private fillContainer: HTMLElement;

    constructor(communication: WebSocketService, element: HTMLElement) {
        super(communication, element);
        this.key = element.getAttribute("name");
        if (this.key == 'L2' || this.key == 'R2') {
            this.fillContainer = document.querySelector(".slider[name=" + this.key + "]");
            this.fillElement = this.fillContainer.querySelector(".fill");
        }
    }
    protected onStart(event: UIInputEvent): number {
        return 1;
    }
    protected onMove(event: UIInputEvent): number {
        return null;
    }
    protected onEnd(): number {
        return 0;
    }
    protected fill(value: number): void {
        if (this.fillElement && this.fillContainer) {
            this.fillElement.style.width = (value * this.fillContainer.offsetWidth) + "px";
        }
    }
    protected sendValue(value: number): void {
        this.communication.sendInput(this.key, value);
    }
}
