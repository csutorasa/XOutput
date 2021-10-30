import { MessageBase } from '../MessageBase';
import { InputDeviceSourceValue } from './InputDeviceSourceValue';
export declare const InputDeviceInputRequestType = "InputDeviceInput";
export declare type InputDeviceInputRequest = MessageBase<typeof InputDeviceInputRequestType> & {
    inputs: InputDeviceSourceValue[];
};
