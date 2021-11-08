import { MessageBase } from '../MessageBase';

export const XboxFeedbackResponseType = 'XboxFeedback';

export interface XboxFeedbackResponse extends MessageBase<typeof XboxFeedbackResponseType> {
  smallForceFeedback: number;
  bigForceFeedback: number;
  ledNumber: number;
}
