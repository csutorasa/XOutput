import { InputDeviceInfo } from '@xoutput/api';
export declare const inputsClient: {
    getInputs(): Promise<InputDeviceInfo[]>;
    getInput(id: string): Promise<InputDeviceInfo>;
};
