import React, { CSSProperties, MouseEvent, TouchEvent, Touch, RefObject } from "react";
import { AbstractInputFlow, UIInputEvent } from "../../events/base";
import { WebSocketService } from "../../communication/websocket";
import { CommonProps } from "./common";


type AxisValue = { x: number, y: number };

export class AxisFlow extends AbstractInputFlow<AxisValue> {
    private key: string;

    constructor(communication: WebSocketService, element: HTMLElement, input: string) {
        super(communication, element);
        this.key = input;
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


export type AxisProp = CommonProps & {
    input: string;
    style: CSSProperties;
}

export class Axis extends React.Component<AxisProp> {

    private element: RefObject<any>;

    constructor(props: Readonly<AxisProp>) {
        super(props);
        this.element = React.createRef();
    }

    private mouseDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new AxisFlow(this.props.websocket, this.element.current, this.props.input), event);
    }

    private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
        Array.from(event.changedTouches).forEach(t => {
            action(t);
        });
        event.preventDefault();
    }

    private touchStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new AxisFlow(this.props.websocket, this.element.current, this.props.input), touch);
        });
    }

    render() {
        return <div ref={this.element} className="twodimensional" style={this.props.style}
            onMouseDown={(event) => this.mouseDown(event)} onTouchStart={(event) => this.touchStart(event)}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">{this.props.input}</div>
            </div>
        </div>;
    }
}