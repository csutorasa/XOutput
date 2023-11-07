import { XboxInputRequest, Emulators, XboxFeedbackResponseType, XboxFeedbackResponse } from '@xoutput/api';
import { websocket, WebSocketSession } from '../websocket';

export type XboxInputMessageSender = {
  sendInput: (input: XboxInputRequest) => void;
};

export const xboxDeviceClient = {
  connect(emulator: Emulators, onFeedback: (feedback: XboxFeedbackResponse) => void): Promise<WebSocketSession<XboxInputMessageSender>> {
    return websocket
      .connect(`MicrosoftXbox360/${emulator}`, (data) => {
        if (data.type === XboxFeedbackResponseType) {
          onFeedback(data as XboxFeedbackResponse);
        }
      })
      .then(
        (session) =>
          ({
            ...session,
            sendInput: (input: XboxInputRequest) => session.sendMessage(input),
          }) as WebSocketSession<XboxInputMessageSender>
      );
  },
};
