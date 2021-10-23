import { MessageBase } from '../MessageBase';

export interface XboxFeedbackResponse extends MessageBase {
  Small: number;
  Large: number;
  LedNumber: number;
}
