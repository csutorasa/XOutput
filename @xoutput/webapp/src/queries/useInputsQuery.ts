import { useQuery, UseQueryResult } from 'react-query';
import { inputsClient } from '@xoutput/client';
import { InputDeviceInfo } from '@xoutput/api';

export const useInputQuery = (id: string): UseQueryResult<InputDeviceInfo> => {
  return useQuery('get-input', () => inputsClient.getInput(id));
};

export const useInputsQuery = (): UseQueryResult<InputDeviceInfo[]> => {
  return useQuery('get-inputs', () => inputsClient.getInputs());
};
