import { MessageBase } from '../MessageBase';
import { InputDeviceSourceValue } from './InputDeviceSourceValue';
import { InputDeviceTargetValue } from './InputDeviceTargetValue';
export declare const InputDeviceInputResponseType = "InputDeviceInputFeedback";
export declare type InputDeviceInputResponse = MessageBase<typeof InputDeviceInputResponseType> & {
    sources: InputDeviceSourceValue[];
    targets: InputDeviceTargetValue[];
};
