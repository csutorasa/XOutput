import {
  InputDeviceDetailsRequest,
  InputDeviceFeedbackResponse,
  InputDeviceFeedbackResponseType,
  InputDeviceInputRequest,
} from '@xoutput/api';
import { websocket, WebSocketSession } from '../websocket';

export type InputDeviceMessageSender = {
  sendInput: (input: InputDeviceInputRequest) => void;
};

export const inputDeviceClient = {
  connect(
    details: InputDeviceDetailsRequest,
    onFeedback: (feedback: InputDeviceFeedbackResponse) => void
  ): Promise<WebSocketSession<InputDeviceMessageSender>> {
    return websocket
      .connect(`InputDevice`, (data) => {
        if (data.type === InputDeviceFeedbackResponseType) {
          onFeedback(data as InputDeviceFeedbackResponse);
        }
      })
      .then((session) => {
        session.sendMessage(details);
        return session;
      })
      .then(
        (session) =>
          ({
            ...session,
            sendInput: (input: InputDeviceInputRequest) => session.sendMessage(input),
          }) as WebSocketSession<InputDeviceMessageSender>
      );
  },
};
