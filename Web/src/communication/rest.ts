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

export type DeviceInfoResponse = DeviceInfo[];

export type DeviceInfo = {
    id: string;
    address: string;
    deviceType: DeviceType;
    emulator: string;
    active: boolean;
    local: boolean;
};

class RestService {
    private http: HttpService;

    constructor(http: HttpService) {
        this.http = http;
    }

    getEmulators(): Promise<ListEmulatorsResponse> {
        return this.http.get<ListEmulatorsResponse>('/emulators');
    }

    getDevices(): Promise<DeviceInfoResponse> {
        return this.http.get<DeviceInfoResponse>('/devices');
    }

    removeDevice(id: string): Promise<DeviceInfoResponse> {
        return this.http.delete<DeviceInfoResponse>(`/devices/${id}`, null);
    }
}

export const rest = new RestService(http);
