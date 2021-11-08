import { CreateMappedControllerRequest, MappedControllerInfo } from '@xoutput/api';
import { http } from '../http';

export const mappedControllersClient = {
  getControllers() {
    return http.get<MappedControllerInfo[]>('/api/mapped/controllers');
  },
  createController(request: CreateMappedControllerRequest) {
    return http.put<CreateMappedControllerRequest>('/api/mapped/controllers', request);
  },
  startController(id: string) {
    return http.put(`/api/mapped/controllers/${id}/active`);
  },
  stopController(id: string) {
    return http.delete(`/api/mapped/controllers/${id}/active`);
  },
  deleteController(id: string) {
    return http.delete(`/api/mapped/controllers/${id}`);
  },
};
