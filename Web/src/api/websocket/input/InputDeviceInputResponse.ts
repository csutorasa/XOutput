import { MessageBase } from "../MessageBase";
import { InputDeviceSourceValue } from "./InputDeviceSourceValue";
import { InputDeviceTargetValue } from "./InputDeviceTargetValue";

export const InputDeviceInputResponseType = "InputDeviceInputFeedback";

export type InputDeviceInputResponse = MessageBase<typeof InputDeviceInputResponseType> & 
{
    sources: InputDeviceSourceValue[];
    targets: InputDeviceTargetValue[];
};
