import React, { CSSProperties, MouseEvent, RefObject, TouchEvent, Touch } from 'react';
import { WebSocketSession } from '../../client/websocket/websocket';
import { CommonProps } from './common';
import { AbstractInputFlow, UIInputEvent } from '../../events/base';

type ButtonValue = boolean | number;

export class ButtonFlow extends AbstractInputFlow<ButtonValue> {
  private key: string;
  private fillContainer: HTMLElement;

  constructor(communication: WebSocketSession, element: HTMLElement, input: string, private emulator: string) {
    super(communication, element);
    this.key = input;
    if (this.key === 'L2' || this.key === 'R2') {
      this.fillContainer = document.querySelector(`.root .slider.${this.key}`);
      this.fillElement = this.fillContainer.querySelector('.fill');
    }
  }
  protected onStart(event: UIInputEvent): ButtonValue {
    if (this.key === 'L2' || this.key === 'R2') {
      return 1;
    }
    return true;
  }
  protected onMove(event: UIInputEvent): boolean {
    return null;
  }
  protected onEnd(): ButtonValue {
    if (this.key === 'L2' || this.key === 'R2') {
      return 0;
    }
    return false;
  }
  protected fill(value: number): void {
    if (this.fillElement && this.fillContainer) {
      this.fillElement.style.width = (value ? 1 : 0) * this.fillContainer.offsetWidth + 'px';
    }
  }
  protected sendValue(value: ButtonValue): void {
    const data: any = {
      Type: this.emulator,
    };
    data[this.key] = value;
    this.communication.sendMessage(data);
  }
}

export type ButtonProp = CommonProps & {
  input: string;
  emulator: string;
  style?: CSSProperties;
  circle?: boolean;
};

export class Button extends React.Component<ButtonProp> {
  private element: RefObject<any>;

  constructor(props: Readonly<ButtonProp>) {
    super(props);
    this.element = React.createRef();
  }

  private mouseDown(event: MouseEvent) {
    this.props.eventHolder.mouseAdd(
      new ButtonFlow(this.props.websocket, this.element.current, this.props.input, this.props.emulator),
      event
    );
  }

  private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
    Array.from(event.changedTouches).forEach((t) => {
      action(t);
    });
    event.preventDefault();
  }

  private touchStart(event: TouchEvent) {
    this.handleTouchEvent(event, (touch) => {
      this.props.eventHolder.touchAdd(
        new ButtonFlow(this.props.websocket, this.element.current, this.props.input, this.props.emulator),
        touch
      );
    });
  }

  render() {
    return (
      <div
        ref={this.element}
        className={this.props.circle ? 'button circle' : 'button'}
        style={this.props.style ? this.props.style : {}}
        onMouseDown={(event) => this.mouseDown(event)}
        onTouchStart={(event) => this.touchStart(event)}
      >
        <div className="inner">{this.props.input}</div>
      </div>
    );
  }
}
