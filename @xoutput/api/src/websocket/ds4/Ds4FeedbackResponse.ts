import { MessageBase } from '../MessageBase';

export const Ds4FeedbackResponseType = 'Ds4Feedback';

export interface Ds4FeedbackResponse extends MessageBase<typeof Ds4FeedbackResponseType> {
  smallForceFeedback: number;
  bigForceFeedback: number;
}
