import { EmulatedControllerInfo } from '@xoutput/api';
import { http } from '../http';

export const emulatedControllersClient = {
  getControllers() {
    return http.get<EmulatedControllerInfo[]>('/emulated/controllers');
  },
  deleteController(id: string) {
    return http.delete(`/emulated/controllers/${id}`);
  },
};
