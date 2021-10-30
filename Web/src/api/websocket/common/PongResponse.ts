import { MessageBase } from '../MessageBase';

export interface PongResponse extends MessageBase<'Pong'> {
  timestamp: number;
}
