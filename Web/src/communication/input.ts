import { MessageBase } from "./message";

export interface InputValues extends MessageBase {
    Values: { [offset: string]: number };
}