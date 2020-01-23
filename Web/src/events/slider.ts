import { WebSocketService } from "../communication/websocket";
import { AbstractInputFlow, UIInputEvent } from "./base";

export class SliderFlow extends AbstractInputFlow<number> {
    private key: string;

    constructor(communication: WebSocketService, element: HTMLElement) {
        super(communication, element);
        this.key = element.getAttribute("name");
    }
    protected onStart(event: UIInputEvent): number {
        return this.getXRatio(event);
    }
    protected onMove(event: UIInputEvent): number {
        return this.getXRatio(event);
    }
    protected onEnd(): number {
        return 0;
    }
    protected fill(value: number): void {
        this.fillElement.style.width = (value * this.element.offsetWidth) + "px";
    }
    protected sendValue(value: number): void {
        this.communication.sendInput(this.key, value);
    }
}
