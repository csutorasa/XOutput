import { MessageBase } from '../MessageBase';

export interface Ds4FeedbackResponse extends MessageBase<'Ds4Feedback'> {
  SmallForceFeedback: number;
  BigForceFeedback: number;
}
