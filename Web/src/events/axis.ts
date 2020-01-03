import { Communication } from "../communication";
import { UIInputEvent, AbstractInputFlow } from "./base";

type AxisValue = { x: number, y: number };

export class AxisFlow extends AbstractInputFlow<AxisValue> {
    private key: string;

    constructor(communication: Communication, element: HTMLElement) {
        super(communication, element);
        this.key = element.getAttribute("name");
    }
    protected onStart(event: UIInputEvent): AxisValue {
        return {
            x: this.getXRatio(event),
            y: this.getYRatio(event),
        };
    }
    protected onMove(event: UIInputEvent): AxisValue {
        return {
            x: this.getXRatio(event),
            y: this.getYRatio(event),
        };
    }
    protected onEnd(): AxisValue {
        return {
            x: 0.5,
            y: 0.5,
        };
    }
    protected fill(value: AxisValue): void {
        const { x, y } = value;
        this.fillElement.style.width = (Math.abs(x - 0.5) * this.element.offsetWidth) + "px";
        this.fillElement.style.left = (Math.min(x, 0.5) * this.element.offsetWidth) + "px";
        this.fillElement.style.height = (Math.abs(y - 0.5) * this.element.offsetHeight) + "px";
        this.fillElement.style.top = (Math.min(y, 0.5) * this.element.offsetHeight) + "px";
    }
    protected sendValue(value: AxisValue): void {
        this.communication.sendInputs(this.key + 'X', value.x, this.key + "Y", 1 - value.y);
    }
}
