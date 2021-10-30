import { InputDeviceInputResponse, InputDeviceInputResponseType } from '../../../api';
import { websocket } from '../websocket';

export const inputDeviceFeedbackClient = {
  connect(id: string, onFeedback: (feedback: InputDeviceInputResponse) => void): Promise<void> {
    return websocket
      .connect(`InputDevice/${id}`, (data) => {
        if (data.type === InputDeviceInputResponseType) {
          onFeedback(data as InputDeviceInputResponse);
        }
      })
      .then(() => null);
  },
};
