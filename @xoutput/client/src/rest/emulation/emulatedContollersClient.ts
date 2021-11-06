import { ControllerInfo, CreateControllerRequest } from '@xoutput/api';
import { http } from '../http';

export const emulatedContollersClient = {
  getControllers() {
    return http.get<ControllerInfo[]>('/api/controllers');
  },
  createController(request: CreateControllerRequest) {
    return http.put<CreateControllerRequest>('/api/controllers', request);
  },
  startController(id: string) {
    return http.put(`/api/controllers/${id}/active`);
  },
  stopController(id: string) {
    return http.delete(`/api/controllers/${id}/active`);
  },
  deleteController(id: string) {
    return http.delete(`/api/controllers/${id}`);
  },
};
