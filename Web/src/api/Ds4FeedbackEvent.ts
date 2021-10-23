import { MessageBase } from './MessageBase';

export interface Ds4FeedbackEvent extends MessageBase {
  Small: number;
  Large: number;
}
