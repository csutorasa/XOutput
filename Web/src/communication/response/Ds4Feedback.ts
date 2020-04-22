import { MessageBase } from "../message";

export interface Ds4Feedback extends MessageBase {
    Small: number;
    Large: number;
}