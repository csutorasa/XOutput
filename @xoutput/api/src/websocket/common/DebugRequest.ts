import { MessageBase } from '../MessageBase';

export const DebugRequestType = 'Ping';

export interface DebugRequest extends MessageBase<typeof DebugRequestType> {
  data: string;
}
