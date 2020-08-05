import { MessageBase } from "./message";
import { InputMethod } from "./rest";

export interface InputValues extends MessageBase {
    Values: InputValue[];
}

export interface InputValue {
    Offset: string;
    Method: InputMethod;
    Value: number;
}
