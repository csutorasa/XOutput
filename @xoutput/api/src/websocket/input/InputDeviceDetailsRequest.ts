import { MessageBase } from '../MessageBase';
import { InputDeviceSource } from '../../common/input/InputDeviceSource';
import { InputDeviceTarget } from '../../common/input/InputDeviceTarget';
import { InputDeviceApi } from '../../common/input/InputDeviceApi';

export const InputDeviceDetailsRequestType = 'InputDeviceDetails';

export type InputDeviceDetailsRequest = MessageBase<typeof InputDeviceDetailsRequestType> & {
  id: string;
  name: string;
  hardwareId?: string;
  sources: InputDeviceSource[];
  targets: InputDeviceTarget[];
  inputApi: InputDeviceApi;
};
