import { Emulators } from '../../common/Emulators';
import { DeviceTypes } from '../../common/DeviceTypes';

export type MappedControllerInfo = {
  id: string;
  name: string;
  deviceType: DeviceTypes;
  emulator: Emulators;
  active: boolean;
};
