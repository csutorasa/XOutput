import { DeviceTypes } from '../../common/DeviceTypes';

export type CreateControllerRequest = {
  name: string;
  deviceType: DeviceTypes;
};
