import { InputDeviceDetailsRequest, InputDeviceFeedbackResponse, InputDeviceInputRequest } from '@xoutput/api';
export declare type InputDeviceMessageSender = {
    sendInput: (input: InputDeviceInputRequest) => void;
};
export declare const inputDeviceClient: {
    connect(details: InputDeviceDetailsRequest, onFeedback: (feedback: InputDeviceFeedbackResponse) => void): Promise<InputDeviceMessageSender>;
};
