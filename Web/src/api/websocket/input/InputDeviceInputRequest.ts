import { MessageBase } from '..';
import { InputDeviceSourceValue } from './InputDeviceSourceValue';

export const InputDeviceInputRequestType = 'InputDeviceInput';

export type InputDeviceInputRequest = MessageBase<typeof InputDeviceInputRequestType> & {
  inputs: InputDeviceSourceValue[];
};
