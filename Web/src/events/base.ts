import { WebSocketService } from "../communication/websocket";

export type UIInputEvent = Touch | MouseEvent;

export interface UIInputFlow {
    start(event: UIInputEvent): void;
    move(event: UIInputEvent): void;
    end(): void;
}

export abstract class AbstractInputFlow<T> implements UIInputFlow {

    protected fillElement: HTMLElement;

    protected constructor(protected communication: WebSocketService, protected element: HTMLElement) {
        this.fillElement = element.querySelector('.fill');
    }

    public start(event: UIInputEvent): void {
        const value = this.onStart(event);
        if (value != null) {
            this.fill(value);
            this.sendValue(value);
        }
    }

    public move(event: UIInputEvent): void {
        const value = this.onMove(event);
        if (value != null) {
            this.fill(value);
            this.sendValue(value);
        }
    }

    public end(): void {
        const value = this.onEnd();
        if (value != null) {
            this.fill(value);
            this.sendValue(value);
        }
    }

    protected abstract onStart(event: UIInputEvent): T;
    protected abstract onMove(event: UIInputEvent): T;
    protected abstract onEnd(): T;
    protected abstract fill(value: T): void;
    protected abstract sendValue(value: T): void;

    protected getXRatio(event: UIInputEvent): number {
        const element = this.element;
        const elementPageX = window.pageXOffset + element.getBoundingClientRect().left;
        const ratio = (event.pageX - elementPageX) / element.offsetWidth;
        return this.normalize(ratio);
    }

    protected getYRatio(event: UIInputEvent): number {
        const element = this.element;
        const elementPageY = window.pageYOffset + element.getBoundingClientRect().top;
        const ratio = (event.pageY - elementPageY) / element.offsetHeight;
        return this.normalize(ratio);
    }

    private normalize(value: number): number {
        if (value > 1) {
            return 1;
        }
        if (value < 0) {
            return 0;
        }
        return value;
    }
}
