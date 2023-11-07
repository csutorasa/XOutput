import {
  InputDeviceDetailsRequest,
  InputDeviceDetailsRequestType,
  InputDeviceFeedbackResponse,
  InputDeviceInputRequestType,
  SourceTypes,
  TargetTypes,
} from '@xoutput/api';
import { inputDeviceClient, InputDeviceMessageSender, WebSocketSession } from '@xoutput/client';

const idPrefix = Math.round(Math.random() * 900_000_000 + 100_000_000).toString(36);

export class GamepadReader {
  private intervalId: NodeJS.Timeout;
  private session: WebSocketSession<InputDeviceMessageSender>;

  constructor(public gamepad: Gamepad) {}

  start(intervalMs = 10): Promise<void> {
    if (this.intervalId) {
      return;
    }
    const buttons = this.gamepad.buttons.map((_, i) => ({
      id: i,
      name: `B${i}`,
      type: 'Button' as SourceTypes,
    }));
    const axes = this.gamepad.axes.map((_, i) => ({
      id: 1000 + i,
      name: `A${i}`,
      type: 'AxisX' as SourceTypes,
    }));
    const sources = buttons.concat(axes);
    const targets = this.gamepad.hapticActuators.map((_, i) => ({
      id: i,
      name: `${i}`,
      type: 'ForceFeedback' as TargetTypes,
    }));
    const details: InputDeviceDetailsRequest = {
      type: InputDeviceDetailsRequestType,
      id: `${idPrefix}-${this.gamepad.index}`,
      name: this.gamepad.id,
      sources,
      targets,
      inputApi: 'GamepadApi',
    };
    return inputDeviceClient
      .connect(details, (feedBack: InputDeviceFeedbackResponse) => {
        feedBack.targets.forEach(() => {
          // this.gamepad.hapticActuators[target.id] = target.value;
        });
      })
      .then((session) => {
        this.session = session;
        this.intervalId = setInterval(() => {
          const buttons = this.gamepad.buttons.map((button, i) => ({
            id: i,
            value: button.value,
          }));
          const axes = this.gamepad.axes.map((axis, i) => ({
            id: 1000 + i,
            value: axis,
          }));
          session.sendInput({
            type: InputDeviceInputRequestType,
            inputs: buttons.concat(axes),
          });
        }, intervalMs);
      });
  }

  isRunning(): boolean {
    return !!this.intervalId;
  }

  stop(): void {
    if (this.intervalId) {
      this.session.close();
      this.session = undefined;
      clearInterval(this.intervalId);
      this.intervalId = undefined;
    }
  }
}
