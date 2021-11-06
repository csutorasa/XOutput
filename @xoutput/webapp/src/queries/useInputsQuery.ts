import { useQuery, UseQueryResult } from 'react-query';
import { emulatedControllersClient, inputsClient } from '@xoutput/client';
import { ControllerInfo, InputDeviceInfo } from '@xoutput/api';

export const useInputsQuery = (): UseQueryResult<InputDeviceInfo[]> => {
  return useQuery('get-inputs', () => inputsClient.getInputs());
};
