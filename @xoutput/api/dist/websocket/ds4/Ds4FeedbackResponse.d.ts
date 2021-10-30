import { MessageBase } from '../MessageBase';
export interface Ds4FeedbackResponse extends MessageBase<'Ds4Feedback'> {
    smallForceFeedback: number;
    bigForceFeedback: number;
}
