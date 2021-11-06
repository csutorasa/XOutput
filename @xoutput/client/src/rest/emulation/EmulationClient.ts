import { EmulatorResponse } from '@xoutput/api';
import { http } from '../http';

export const emulationClient = {
  getInfo(): Promise<EmulatorResponse> {
    return http.get<EmulatorResponse>('/emulators');
  },
};
