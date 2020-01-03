import { Communication } from "../communication";
import { AbstractInputFlow, UIInputEvent } from "./base";

type DPadValue = { up: number, down: number, left: number, right: number };

export class DPadFlow extends AbstractInputFlow<DPadValue> {
    private key: string;

    constructor(communication: Communication, element: HTMLElement) {
        super(communication, element);
        this.key = element.getAttribute("name");
    }
    protected onStart(event: UIInputEvent): DPadValue {
        const xValue = this.getXRatio(event);
        const yValue = this.getYRatio(event);
        return {
            up: yValue < 0.25 ? 1 : 0,
            down: yValue > 0.75 ? 1 : 0,
            left: xValue < 0.25 ? 1 : 0,
            right: xValue > 0.75 ? 1 : 0,
        };
    }
    protected onMove(event: UIInputEvent): DPadValue {
        const xValue = this.getXRatio(event);
        const yValue = this.getYRatio(event);
        return {
            up: yValue < 0.25 ? 1 : 0,
            down: yValue > 0.75 ? 1 : 0,
            left: xValue < 0.25 ? 1 : 0,
            right: xValue > 0.75 ? 1 : 0,
        };
    }
    protected onEnd(): DPadValue {
        return {
            up: 0,
            down: 0,
            left: 0,
            right: 0,
        };
    }
    protected fill(value: DPadValue): void {
        const { up, down, left, right } = value;
        this.fillElement.style.left = (0.375 * this.element.offsetWidth) + "px";
        this.fillElement.style.top = (0.375 * this.element.offsetHeight) + "px";
        if (up || down || left || right) {
            this.fillElement.style.width = (0.25 * this.element.offsetWidth) + "px";
            this.fillElement.style.height = (0.25 * this.element.offsetHeight) + "px";
            if (up) {
                this.fillElement.style.top = "0px";
            } else if (down) {
                this.fillElement.style.top = (0.75 * this.element.offsetHeight) + "px";
            }
            if (left) {
                this.fillElement.style.left = "0px";
            } else if (right) {
                this.fillElement.style.left = (0.75 * this.element.offsetWidth) + "px";
            }
        } else {
            this.fillElement.style.width = "0px";
            this.fillElement.style.height = "0px";
        }
    }
    protected sendValue(value: DPadValue): void {
        this.communication.sendDPad(value.up, value.down, value.left, value.right);
    }
}
