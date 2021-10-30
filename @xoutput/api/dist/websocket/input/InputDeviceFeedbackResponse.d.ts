import { MessageBase } from '../MessageBase';
import { InputDeviceTargetValue } from './InputDeviceTargetValue';
export declare const InputDeviceFeedbackResponseType = "InputDeviceFeedback";
export declare type InputDeviceFeedbackResponse = MessageBase<typeof InputDeviceFeedbackResponseType> & {
    targets: InputDeviceTargetValue[];
};
