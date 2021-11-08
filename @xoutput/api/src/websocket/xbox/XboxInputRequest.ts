import { MessageBase } from '../MessageBase';

export const XboxInputRequestType = 'XboxInput';

export interface XboxInputRequest extends MessageBase<typeof XboxInputRequestType> {
  a: boolean;
  b: boolean;
  x: boolean;
  y: boolean;
  l1: boolean;
  l3: boolean;
  r1: boolean;
  r3: boolean;
  start: boolean;
  back: boolean;
  home: boolean;
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
