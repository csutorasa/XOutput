import { InputDeviceInfo } from '../../../api';
import { http } from '../http';

export const inputsClient = {
    getInputs(): Promise<InputDeviceInfo[]> {
        return http.get<InputDeviceInfo[]>("/inputs");
    },
    getInput(id: string): Promise<InputDeviceInfo> {
        return http.get<InputDeviceInfo>(`/inputs/${id}`);
    },
}
