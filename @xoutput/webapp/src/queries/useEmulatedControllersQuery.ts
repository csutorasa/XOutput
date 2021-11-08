import { useQuery, UseQueryResult } from 'react-query';
import { emulatedControllersClient } from '@xoutput/client';
import { EmulatedControllerInfo } from '@xoutput/api';

export const useEmulatedControllersQuery = (): UseQueryResult<EmulatedControllerInfo[]> => {
  return useQuery('get-emulated-controllers', () => emulatedControllersClient.getControllers());
};
