import { ControllerInputResponse, ControllerInputResponseType } from '@xoutput/api';
import { websocket, WebSocketSession } from '../websocket';

export const inputDeviceClient = {
  connect(id: string, onFeedback: (feedback: ControllerInputResponse) => void): Promise<WebSocketSession> {
    return websocket.connect(`EmulatedController/${id}`, (data) => {
      if (data.type === ControllerInputResponseType) {
        onFeedback(data as ControllerInputResponse);
      }
    });
  },
};
