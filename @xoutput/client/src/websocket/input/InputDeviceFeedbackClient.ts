import { InputDeviceInputResponse, InputDeviceInputResponseType } from '@xoutput/api';
import { websocket, WebSocketSession } from '../websocket';

export const inputDeviceFeedbackClient = {
  connect(id: string, onFeedback: (feedback: InputDeviceInputResponse) => void): Promise<WebSocketSession> {
    return websocket.connect(`InputDevice/${id}`, (data) => {
      if (data.type === InputDeviceInputResponseType) {
        onFeedback(data as InputDeviceInputResponse);
      }
    });
  },
};
