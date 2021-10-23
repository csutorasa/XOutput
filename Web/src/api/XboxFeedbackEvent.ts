import { MessageBase } from './MessageBase';

export interface XboxFeedbackEvent extends MessageBase {
  Small: number;
  Large: number;
  LedNumber: number;
}
