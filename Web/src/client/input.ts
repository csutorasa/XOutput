import { MessageBase } from '../api/websocket/MessageBase';
import { InputMethod } from './rest';

export interface InputValues extends MessageBase {
  Values: InputValue[];
}

export interface InputValue {
  Offset: string;
  Method: InputMethod;
  Value: number;
}
