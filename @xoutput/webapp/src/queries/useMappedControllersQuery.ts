import { useQuery, UseQueryResult } from 'react-query';
import { mappedControllersClient } from '@xoutput/client';
import { MappedControllerInfo } from '@xoutput/api';

export const useMappedControllersQuery = (): UseQueryResult<MappedControllerInfo[]> => {
  return useQuery('get-mapped-controllers', () => mappedControllersClient.getControllers());
};
