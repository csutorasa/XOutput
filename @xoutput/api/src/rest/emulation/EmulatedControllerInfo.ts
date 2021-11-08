import { Emulators } from '../../common/Emulators';
import { DeviceTypes } from '../../common/DeviceTypes';

export type EmulatedControllerInfo = {
  id: string;
  address?: string;
  name: string;
  deviceType: DeviceTypes;
  emulator: Emulators;
  active: boolean;
};
