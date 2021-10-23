import { MessageBase } from '../MessageBase';

export interface DebugRequest extends MessageBase<'Debug'> {
  Data: string;
}
