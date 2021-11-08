import { DeviceTypes } from '../../common/DeviceTypes';

export type CreateMappedControllerRequest = {
  name: string;
  deviceType: DeviceTypes;
};
