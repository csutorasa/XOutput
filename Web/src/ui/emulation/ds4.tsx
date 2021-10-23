import React, { MouseEvent, ReactElement } from 'react';
import { Dpad } from './dpad';
import { Slider } from './slider';
import { Button } from './button';
import { Axis } from './axis';
import CircularProgress from '@material-ui/core/CircularProgress';
import { ListEmulatorsResponse, rest, EmulatorResponse } from '../../client/rest';
import { WebSocketService, WebSocketSession } from '../../client/websocket/websocket';
import { Square } from './square';
import { EventHolder } from '../../events/eventholder';
import { TranslatedText as TT } from '../TranslatedText';
import { MessageBase } from '../../api/websocket/MessageBase';
import { Ds4FeedbackResponse } from '../../api/websocket/ds4/Ds4FeedbackResponse';

interface EmulationState {
  emulators: ListEmulatorsResponse;
  loading: boolean;
}

export interface EmulationProps {
  deviceType: string;
  emulator: string;
}

export class Ds4Emulation extends React.Component<EmulationProps, EmulationState, any> {
  private eventHolder: EventHolder;
  private websocket: WebSocketService;
  private websocketSession: WebSocketSession;
  private mouseMove = (event: Event) => this.eventHolder.mouseMove(event as any);
  private mouseUp = () => this.eventHolder.mouseEnd();
  private touchMove = (event: TouchEvent) => this.handleTouchEvent(event, (t: Touch) => this.eventHolder.touchMove(t));
  private touchEnd = (event: TouchEvent) => this.handleTouchEvent(event, (t) => this.eventHolder.touchEnd(t));
  private touchCancel = () => this.eventHolder.touchEndAll();

  constructor(props: Readonly<any>) {
    super(props);
    this.state = {
      emulators: null,
      loading: true,
    };
  }

  private handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
    Array.from(event.changedTouches).forEach((t) => {
      action(t);
    });
    event.preventDefault();
  }

  componentDidMount() {
    this.eventHolder = new EventHolder();
    this.websocket = new WebSocketService();
    this.websocket
      .connect(`${this.props.deviceType}/${this.props.emulator}`, (data) => this.onData(data))
      .then((s) => (this.websocketSession = s))
      .then(() => {
        document.addEventListener('mousemove', this.mouseMove, false);
        document.addEventListener('mouseup', this.mouseUp, false);
        document.addEventListener('touchmove', this.touchMove, false);
        document.addEventListener('touchend', this.touchEnd, false);
        document.addEventListener('touchcancel', this.touchCancel, false);
        this.setState((state, props) => {
          return {
            emulators: state.emulators,
            loading: false,
          };
        });
      });
    document.querySelector('html').classList.add('fullscreen');
  }

  componentWillUnmount() {
    document.removeEventListener('mousemove', this.mouseMove, false);
    document.removeEventListener('mouseup', this.mouseUp, false);
    document.removeEventListener('touchmove', this.touchMove, false);
    document.removeEventListener('touchend', this.touchEnd, false);
    document.removeEventListener('touchcancel', this.touchCancel, false);
    this.websocketSession.close();
    document.querySelector('html').classList.remove('fullscreen');
  }

  openFullscreen() {
    document
      .querySelector('.root')
      .requestFullscreen()
      .catch((err) => {
        this.websocketSession.sendDebug(err);
      });
  }

  private onData(data: MessageBase) {
    if (data.Type === 'Ds4Feedback') {
      const feedback = data as Ds4FeedbackResponse;
      if (navigator.vibrate) {
        if (feedback.Small > 0 || feedback.Large > 0) {
          navigator.vibrate(60 * 60 * 1000);
        } else {
          navigator.vibrate(0);
        }
      }
    }
  }

  render() {
    if (this.state.loading) {
      return <CircularProgress />;
    }
    const emulator = 'Ds4Input';
    return (
      <div className="root">
        <Slider
          input="L2"
          style={{ gridColumn: 'span 10', gridRow: 'span 5' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Slider>
        <Slider
          input="R2"
          style={{ gridColumn: 'span 10', gridRow: 'span 5' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
          inverted={true}
        ></Slider>

        <Dpad
          style={{ gridColumn: 'span 7', gridRow: 'span 8' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Dpad>
        <Square style={{ gridColumn: 'span 7', gridRow: 'span 8' }}>
          <div>
            <Button
              input="Share"
              circle={true}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div>
            <div className="fullscreen button circle" onClick={() => this.openFullscreen()} onTouchStart={() => this.openFullscreen()}>
              <div className="inner">
                <TT text="Fullscreen" />
              </div>
            </div>
          </div>
          <div>
            <Button
              input="Options"
              circle={true}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div></div>
          <div>
            <Button input="Ps" circle={true} eventHolder={this.eventHolder} websocket={this.websocketSession} emulator={emulator}></Button>
          </div>
          <div></div>
          <div>
            <Button input="L3" circle={true} eventHolder={this.eventHolder} websocket={this.websocketSession} emulator={emulator}></Button>
          </div>
          <div></div>
          <div>
            <Button input="R3" circle={true} eventHolder={this.eventHolder} websocket={this.websocketSession} emulator={emulator}></Button>
          </div>
        </Square>
        <Square style={{ gridColumn: 'span 6', gridRow: 'span 8' }}>
          <div></div>
          <div>
            <Button
              input="Triangle"
              circle={true}
              style={{ backgroundColor: 'yellow', color: 'orangered' }}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div></div>
          <div>
            <Button
              input="Cross"
              circle={true}
              style={{ backgroundColor: 'blue', color: 'lightblue' }}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div></div>
          <div>
            <Button
              input="Circle"
              circle={true}
              style={{ backgroundColor: 'red', color: 'darkred' }}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div></div>
          <div>
            <Button
              input="Square"
              circle={true}
              style={{ backgroundColor: 'green', color: 'lightgreen' }}
              eventHolder={this.eventHolder}
              websocket={this.websocketSession}
              emulator={emulator}
            ></Button>
          </div>
          <div></div>
        </Square>

        <Button
          input="L1"
          style={{ gridColumn: 'span 4', gridRow: 'span 7' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Button>
        <Axis
          input="L"
          style={{ gridColumn: 'span 6', gridRow: 'span 7' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Axis>
        <Axis
          input="R"
          style={{ gridColumn: 'span 6', gridRow: 'span 7' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Axis>
        <Button
          input="R1"
          style={{ gridColumn: 'span 4', gridRow: 'span 7' }}
          eventHolder={this.eventHolder}
          websocket={this.websocketSession}
          emulator={emulator}
        ></Button>
      </div>
    );
  }
}
