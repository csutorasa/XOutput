import { MessageBase } from '../MessageBase';
export interface XboxFeedbackResponse extends MessageBase<'XboxFeedback'> {
    smallForceFeedback: number;
    bigForceFeedback: number;
    ledNumber: number;
}
