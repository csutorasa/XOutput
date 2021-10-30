import { MessageBase } from '../MessageBase';
import { InputDeviceTargetValue } from './InputDeviceTargetValue';

export const InputDeviceFeedbackResponseType = 'InputDeviceFeedback';

export type InputDeviceFeedbackResponse = MessageBase<typeof InputDeviceFeedbackResponseType> & {
  targets: InputDeviceTargetValue[];
};
