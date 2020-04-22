import React, { CSSProperties, RefObject, TouchEvent, Touch, MouseEvent } from "react";
import { UIInputEvent, AbstractInputFlow } from "../../events/base";
import { WebSocketService } from "../../communication/websocket";
import { CommonProps } from "./common";

type DPadValue = { up: number, down: number, left: number, right: number };

export class DPadFlow extends AbstractInputFlow<DPadValue> {

    constructor(communication: WebSocketService, element: HTMLElement, private emulator: string) {
        super(communication, element);
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
        const data: any = {
            Type: this.emulator
        };
        data.UP = value.up;
        data.DOWN = value.down;
        data.LEFT = value.left;
        data.RIGHT = value.right;
        this.communication.sendMessage(data);
    }
}

export type DpadProp = CommonProps & {
    emulator: string;
    style: CSSProperties;
}

export class Dpad extends React.Component<DpadProp> {

    private element: RefObject<any>;

    constructor(props: Readonly<DpadProp>) {
        super(props);
        this.element = React.createRef();
    }

    private mouseDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new DPadFlow(this.props.websocket, this.element.current, this.props.emulator), event);
    }

    private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
        Array.from(event.changedTouches).forEach(t => {
            action(t);
        });
        event.preventDefault();
    }

    private touchStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new DPadFlow(this.props.websocket, this.element.current, this.props.emulator), touch);
        });
    }

    render() {
        return <div ref={this.element} className="dpad" style={this.props.style}
            onMouseDown={(event) => this.mouseDown(event)} onTouchStart={(event) => this.touchStart(event)}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">DPad</div>
            </div>
        </div>;
    }
}