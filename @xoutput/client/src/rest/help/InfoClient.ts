import { InfoResponse } from '@xoutput/api';
import { http } from '../http';

export const infoClient = {
  getInfo(): Promise<InfoResponse> {
    return http.get<InfoResponse>('/info');
  },
};
