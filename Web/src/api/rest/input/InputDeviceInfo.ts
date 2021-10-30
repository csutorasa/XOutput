import { InputDeviceSource, InputDeviceTarget } from '../../common';

export type InputDeviceInfo = {
  id: string;
  name: string;
  deviceApi: string;
  sources: InputDeviceSource[];
  targets: InputDeviceTarget[];
};
