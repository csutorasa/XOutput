import { useQuery, useQueryClient, UseQueryResult } from 'react-query';
import { notificationClient } from '@xoutput/client';
import { Notification } from '@xoutput/api';

export const useNotificationsQuery = (): UseQueryResult<Notification[]> => {
  return useQuery('get-notifications', () => notificationClient.getNotifications());
};

export const resetNotificationsQuery = (): Promise<void> => {
  const client = useQueryClient();
  return client.invalidateQueries('get-notifications');
};
