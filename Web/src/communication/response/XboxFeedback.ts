import { MessageBase } from '../message';

export interface XboxFeedback extends MessageBase {
  Small: number;
  Large: number;
  LedNumber: number;
}
