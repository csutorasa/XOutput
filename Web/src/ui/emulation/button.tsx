import React, { CSSProperties, MouseEvent, RefObject, TouchEvent, Touch } from "react";
import { WebSocketService } from "../../communication/websocket";
import { CommonProps } from "./common";
import { AbstractInputFlow, UIInputEvent } from "../../events/base";

export class ButtonFlow extends AbstractInputFlow<boolean> {
    private key: string;
    private fillContainer: HTMLElement;

    constructor(communication: WebSocketService, element: HTMLElement, input: string) {
        super(communication, element);
        this.key = input;
        if (this.key == 'L2' || this.key == 'R2') {
            this.fillContainer = document.querySelector(`.root .slider .${this.key}`);
            this.fillElement = this.fillContainer.querySelector(".fill");
        }
    }
    protected onStart(event: UIInputEvent): boolean {
        return true;
    }
    protected onMove(event: UIInputEvent): boolean {
        return null;
    }
    protected onEnd(): boolean {
        return false;
    }
    protected fill(value: boolean): void {
        if (this.fillElement && this.fillContainer) {
            this.fillElement.style.width = ((value ? 1 : 0) * this.fillContainer.offsetWidth) + "px";
        }
    }
    protected sendValue(value: boolean): void {
        this.communication.sendInput(this.key, value);
    }
}


export type ButtonProp = CommonProps & {
    input: string;
    style?: CSSProperties;
    circle?: boolean;
}

export class Button extends React.Component<ButtonProp> {

    constructor(props: Readonly<ButtonProp>) {
        super(props);
    }

    private mouseDown(event: MouseEvent) {
        this.props.eventHolder.mouseAdd(new ButtonFlow(this.props.websocket, null, this.props.input), event);
    }
    
    private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
        Array.from(event.changedTouches).forEach(t => {
            action(t);
        });
        event.preventDefault();
    }

    private touchStart(event: TouchEvent) {
        this.handleTouchEvent(event, (touch) => {
            this.props.eventHolder.touchAdd(new ButtonFlow(this.props.websocket, null, this.props.input), touch);
        });
    }

    render() {
        return <div className={this.props.circle ? "button circle" : "button"} style={this.props.style ? this.props.style : {}}
                onMouseDown={(event) => this.mouseDown(event)} onTouchStart={(event) => this.touchStart(event)}>
            <div className="inner">
                {this.props.input}
            </div>
        </div>;
    }
}