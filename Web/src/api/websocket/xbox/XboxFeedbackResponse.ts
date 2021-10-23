import { MessageBase } from '../MessageBase';

export interface XboxFeedbackResponse extends MessageBase<'XboxFeedback'> {
  SmallForceFeedback: number;
  BigForceFeedback: number;
  LedNumber: number;
}
