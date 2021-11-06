import { DeviceTypes } from '../../common/DeviceTypes';

export type EmulatorResponse = {
  installed: boolean;
  SupportedDeviceTypes: DeviceTypes[];
};
