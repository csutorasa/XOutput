import { useQuery, UseQueryResult } from 'react-query';
import { emulatedControllersClient } from '@xoutput/client';
import { ControllerInfo } from '@xoutput/api';

export const useControllersQuery = (): UseQueryResult<ControllerInfo[]> => {
  return useQuery('get-controllers', () => emulatedControllersClient.getControllers());
};
