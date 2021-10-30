import { MessageBase } from '..';
import { InputDeviceTargetValue } from './InputDeviceTargetValue';

export const InputDeviceFeedbackResponseType = 'InputDeviceFeedback';

export type InputDeviceFeedbackResponse = MessageBase<typeof InputDeviceFeedbackResponseType> & {
  targets: InputDeviceTargetValue[];
};
