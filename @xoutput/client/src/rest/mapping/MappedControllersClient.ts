import { CreateMappedControllerRequest, MappedControllerInfo } from '@xoutput/api';
import { http } from '../http';

export const mappedControllersClient = {
  getControllers() {
    return http.get<MappedControllerInfo[]>('/mapped/controllers');
  },
  createController(request: CreateMappedControllerRequest) {
    return http.put<CreateMappedControllerRequest>('/mapped/controllers', request);
  },
  startController(id: string) {
    return http.put(`/mapped/controllers/${id}/active`);
  },
  stopController(id: string) {
    return http.delete(`/mapped/controllers/${id}/active`);
  },
  deleteController(id: string) {
    return http.delete(`/mapped/controllers/${id}`);
  },
};
