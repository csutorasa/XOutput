import { MessageBase } from '../MessageBase';

export const PingRequestType = 'Ping';

export interface PingRequest extends MessageBase<typeof PingRequestType> {
  timestamp: number;
}
