import { http, HttpService } from "./http";

export enum DeviceType {
    MicrosoftXbox360 = 'MicrosoftXbox360',
    SonyDualShock4 = 'SonyDualShock4',
}

export interface EmulatorResponse {
    installed: boolean;
    supportedDeviceTypes: DeviceType[];
}

export interface ListEmulatorsResponse {
    [emulator: string]: EmulatorResponse;
}

export type ControllerInfoResponse = ControllerInfo[];

export type ControllerInfo = {
    id: string;
    address: string;
    deviceType: DeviceType;
    emulator: string;
    active: boolean;
    local: boolean;
};

export type InputDeviceInfoResponse = InputDeviceInformation[];

export type InputDeviceInformation = {
    id: string;
    name: string;
    dPads: number;
    axes: number;
    buttons: number;
    sliders: number;
};

class RestService {
    private http: HttpService;

    constructor(http: HttpService) {
        this.http = http;
    }

    getEmulators(): Promise<ListEmulatorsResponse> {
        return this.http.get<ListEmulatorsResponse>('/emulators');
    }

    getControllers(): Promise<ControllerInfoResponse> {
        return this.http.get<ControllerInfoResponse>('/controllers');
    }

    getInputDevices(): Promise<InputDeviceInfoResponse> {
        return this.http.get<InputDeviceInfoResponse>('/inputs');
    }

    removeControllers(id: string): Promise<ControllerInfoResponse> {
        return this.http.delete<ControllerInfoResponse>(`/controllers/${id}`, null);
    }
}

export const rest = new RestService(http);
