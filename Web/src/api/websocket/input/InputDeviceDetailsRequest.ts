import { MessageBase } from '../MessageBase';
import { InputDeviceSource, InputDeviceTarget, InputDeviceApi } from '../../common';

export const InputDeviceDetailsRequestType = 'InputDeviceDetails';

export type InputDeviceDetailsRequest = MessageBase<typeof InputDeviceDetailsRequestType> & {
  id: string;
  name: string;
  hardwareId?: string;
  sources: InputDeviceSource[];
  targets: InputDeviceTarget[];
  inputApi: InputDeviceApi;
};
