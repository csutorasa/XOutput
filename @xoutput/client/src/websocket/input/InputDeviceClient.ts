import {
  InputDeviceDetailsRequest,
  InputDeviceFeedbackResponse,
  InputDeviceFeedbackResponseType,
  InputDeviceInputRequest,
} from '@xoutput/api';
import { websocket } from '../websocket';

export type InputDeviceMessageSender = {
  sendInput: (input: InputDeviceInputRequest) => void;
};

export const inputDeviceClient = {
  connect(
    details: InputDeviceDetailsRequest,
    onFeedback: (feedback: InputDeviceFeedbackResponse) => void
  ): Promise<InputDeviceMessageSender> {
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
      .then((session) => ({
        sendInput: (input: InputDeviceInputRequest) => session.sendMessage(input),
      }));
  },
};
