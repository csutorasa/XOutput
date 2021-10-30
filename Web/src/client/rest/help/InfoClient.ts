import { InfoResponse } from '../../../api';
import { http } from '../http';

export const infoClient = {
  getInfo(): Promise<InfoResponse> {
    return http.get<InfoResponse>('/info');
  },
};
