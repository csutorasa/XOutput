import { MessageBase } from '../MessageBase';
import { ControllerSourceValue } from './ControllerSourceValue';
import { ControllerTargetValue } from './ControllerTargetValue';

export const ControllerInputResponseType = 'ControllerInputFeedback';

export interface ControllerInputResponse extends MessageBase<typeof ControllerInputResponseType> {
  sources: ControllerSourceValue[];
  targets: ControllerTargetValue[];
}
