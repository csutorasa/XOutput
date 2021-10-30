import { Notification } from '@xoutput/api';
export declare const notificationClient: {
    getNotifications(): Promise<Notification[]>;
    acknowledge(id: string): Promise<void>;
};
