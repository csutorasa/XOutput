import { MessageBase } from '../MessageBase';

export const Ds4InputRequestType = 'Ds4Input';

export interface Ds4InputRequest extends MessageBase<typeof Ds4InputRequestType> {
  circle: boolean;
  cross: boolean;
  triangle: boolean;
  square: boolean;
  l1: boolean;
  l3: boolean;
  r1: boolean;
  r3: boolean;
  options: boolean;
  share: boolean;
  ps: boolean;
  up: boolean;
  down: boolean;
  left: boolean;
  right: boolean;
  lX: number;
  lY: number;
  rX: number;
  rY: number;
  l2: number;
  r2: number;
}
