import { InputDeviceInputResponse } from '@xoutput/api';
export declare const inputDeviceFeedbackClient: {
    connect(id: string, onFeedback: (feedback: InputDeviceInputResponse) => void): Promise<void>;
};
