import { MessageBase } from '../MessageBase';

export const PongResponseType = 'Pong';

export interface PongResponse extends MessageBase<typeof PongResponseType> {
  timestamp: number;
}
