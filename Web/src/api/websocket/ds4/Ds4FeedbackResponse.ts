import { MessageBase } from '../MessageBase';

export interface Ds4FeedbackResponse extends MessageBase {
  Small: number;
  Large: number;
}
