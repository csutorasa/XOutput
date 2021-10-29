import { Notification } from '../../../api';
import { http } from '../http';

export const notificationClient = {
    getNotifications(): Promise<Notification[]> {
        return http.get<Notification[]>("/notifications");
    },
    acknowledge(id: string): Promise<void> {
        return http.put<void>(`/notifications/${id}/acknowledge`);
    },
}
