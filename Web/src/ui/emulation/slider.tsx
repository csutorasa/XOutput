import React, { CSSProperties, TouchEvent, MouseEvent, Touch, RefObject } from "react";
import { WebSocketService } from "../../communication/websocket";
import { AbstractInputFlow, UIInputEvent } from "../../events/base";
import { CommonProps } from "./common";
import { ButtonFlow } from "./button";

export class SliderFlow extends AbstractInputFlow<number> {
    private key: string;

    constructor(communication: WebSocketService, element: HTMLElement, input: string, private inverted: boolean, private emulator: string) {
        super(communication, element);
        this.key = input;
    }
    protected onStart(event: UIInputEvent): number {
        return this.getXRatio(event, this.inverted);
    }
    protected onMove(event: UIInputEvent): number {
        return this.getXRatio(event, this.inverted);
    }
    protected onEnd(): number {
        return 0;
    }
    protected fill(value: number): void {
        this.fillElement.style.width = (value * this.element.offsetWidth) + "px";
    }
    protected sendValue(value: number): void {
        const data: any = {
            Type: this.emulator
        };
        data[this.key] = value;
        this.communication.sendMessage(data);
    }
}

export type SliderProp = CommonProps & {
    input: string;
    emulator: string;
    style: CSSProperties;
    inverted?: boolean;
}

export class Slider extends React.Component<SliderProp> {

    private element: RefObject<any>;

    constructor(props: Readonly<SliderProp>) {
        super(props);
        this.element = React.createRef();
    }

    private mouseDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new SliderFlow(this.props.websocket, this.element.current, this.props.input, this.props.inverted, this.props.emulator), event);
    }

    private mouseButtonDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new ButtonFlow(this.props.websocket, this.element.current, this.props.input, this.props.emulator), event);
    }

    private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
        Array.from(event.changedTouches).forEach(t => {
            action(t);
        });
        event.preventDefault();
    }

    private touchStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new SliderFlow(this.props.websocket, this.element.current, this.props.input, this.props.inverted, this.props.emulator), touch);
        });
    }

    private touchButtonStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new ButtonFlow(this.props.websocket, this.element.current, this.props.input, this.props.emulator), touch);
        });
    }

    render() {
        const button = <div className="button" style={{ gridColumn: "span 4" }}
            onMouseDown={(event) => this.mouseButtonDown(event)} onTouchStart={(event) => this.touchButtonStart(event)}>
            <div className="inner">
                {this.props.input}
            </div>
        </div>;
        const slider = <div ref={this.element} className={`slider ${this.props.input}`} style={{ gridColumn: "span 6" }}
            onMouseDown={(event) => this.mouseDown(event)} onTouchStart={(event) => this.touchStart(event)}>
            <div className={"fill" + (this.props.inverted ? " inverted" : "")}></div>
            <div className="text">
                <div className="inner">{this.props.input}</div>
            </div>
        </div>;
        if (this.props.inverted) {
            return <div className="slider-container" style={this.props.style ? this.props.style : {}}>
                {slider}
                {button}
            </div>;
        }
        return <div className="slider-container" style={this.props.style ? this.props.style : {}}>
            {button}
            {slider}
        </div>;
    }
}