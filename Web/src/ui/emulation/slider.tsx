import React, { CSSProperties, TouchEvent, MouseEvent, Touch, RefObject } from "react";
import { WebSocketService } from "../../communication/websocket";
import { AbstractInputFlow, UIInputEvent } from "../../events/base";
import { CommonProps } from "./common";

export class SliderFlow extends AbstractInputFlow<number> {
    private key: string;

    constructor(communication: WebSocketService, element: HTMLElement, input: string) {
        super(communication, element);
        this.key = input;
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

export type SliderProp = CommonProps & {
    input: string;
    style: CSSProperties;
}

export class Slider extends React.Component<SliderProp> {

    private element: RefObject<any>;

    constructor(props: Readonly<SliderProp>) {
        super(props);
        this.element = React.createRef();
    }

    private mouseDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new SliderFlow(this.props.websocket, this.element.current, this.props.input), event);
    }

    private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
        Array.from(event.changedTouches).forEach(t => {
            action(t);
        });
        event.preventDefault();
    }

    private touchStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new SliderFlow(this.props.websocket, this.element.current, this.props.input), touch);
        });
    }

    render() {
        return <div ref={this.element} className={`slider ${this.props.input}`} style={this.props.style}
            onMouseDown={(event) => this.mouseDown(event)} onTouchStart={(event) => this.touchStart(event)}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">{this.props.input}</div>
            </div>
        </div>;
    }
}