export declare type NotificationLevel = 'Info' | 'Warn' | 'Error';
export declare type Notification = {
    id: string;
    acknowledged: boolean;
    createdAt: string;
    key: string;
    level: string;
    parameters: string[];
};
