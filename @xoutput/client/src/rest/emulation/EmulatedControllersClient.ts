import { EmulatedControllerInfo } from '@xoutput/api';
import { http } from '../http';

export const emulatedControllersClient = {
  getControllers() {
    return http.get<EmulatedControllerInfo[]>('/api/emulated/controllers');
  },
  deleteController(id: string) {
    return http.delete(`/api/emulated/controllers/${id}`);
  },
};
