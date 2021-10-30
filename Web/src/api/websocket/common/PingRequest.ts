import { MessageBase } from '../MessageBase';

export interface PingRequest extends MessageBase<'Ping'> {
  timestamp: number;
}
