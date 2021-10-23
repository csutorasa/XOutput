import { MessageBase } from '../MessageBase';

export interface PongResponse extends MessageBase<'Pong'> {
  Timestamp: number;
}
