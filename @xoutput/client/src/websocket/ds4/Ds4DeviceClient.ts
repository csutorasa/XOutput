import { Ds4InputRequest, Emulators, Ds4FeedbackResponseType, Ds4FeedbackResponse } from '@xoutput/api';
import { websocket, WebSocketSession } from '../websocket';

export type Ds4InputMessageSender = {
  sendInput: (input: Ds4InputRequest) => void;
};

export const ds4DeviceClient = {
  connect(emulator: Emulators, onFeedback: (feedback: Ds4FeedbackResponse) => void): Promise<WebSocketSession<Ds4InputMessageSender>> {
    return websocket
      .connect(`SonyDualShock4/${emulator}`, (data) => {
        if (data.type === Ds4FeedbackResponseType) {
          onFeedback(data as Ds4FeedbackResponse);
        }
      })
      .then(
        (session) =>
          ({
            ...session,
            sendInput: (input: Ds4InputRequest) => session.sendMessage(input),
          }) as WebSocketSession<Ds4InputMessageSender>
      );
  },
};
