export type NotificationLevel = 'Info' | 'Warn' | 'Error';

export type Notification = {
  id: string;
  acknowledged: boolean;
  createdAt: string;
  key: string;
  level: string;
  parameters: string[];
};
