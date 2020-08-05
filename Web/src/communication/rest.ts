import { http, HttpService } from "./http";

export enum InputMethod {
    WindowsApi = 'WindowsApi',
    DirectInput = 'DirectInput',
    RawInput = 'RawInput',
    Websocket = 'Websocket',
}

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
    activeFeatures: string[];
};

export type InputDeviceDetails = {
    id: string;
    name: string;
    hardwareId: string;
    inputs: InputDeviceInputDetails[];
};

export type InputDeviceInputDetails = {
    running: boolean;
    sources: {
        offset: number;
        name: string;
        type: 'button' | 'slider' | 'dpad' | 'axis';
    }[];
    forceFeedbacks: {
        offset: number;
    }[];
    inputMethod: InputMethod;
};

export type InputDeviceConfig = {
    bigMotors: number[];
    smallMotors: number[];
};

export type HidGuardianInfo = {
    available: boolean;
    active: boolean;
};

export type Notification = {
    id: string;
    key: string;
    acknowledged: boolean;
    level: string;
    parameters: string[];
    createdAt: string;
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

    getInputDeviceDetails(id: string): Promise<InputDeviceDetails> {
        return this.http.get<InputDeviceDetails>(`/inputs/${id}`);
    }

    getInputDeviceConfig(id: string): Promise<InputDeviceConfig> {
        return this.http.get<InputDeviceConfig>(`/inputs/${id}/configuration`);
    }

    addInputDeviceConfig(id: string, config: string, offset: number): Promise<void> {
        return this.http.put<void>(`/inputs/${id}/configuration/${config}/${offset}`);
    }

    removeInputDeviceConfig(id: string, config: string, offset: number): Promise<void> {
        return this.http.delete<void>(`/inputs/${id}/configuration/${config}/${offset}`);
    }

    removeControllers(id: string): Promise<ControllerInfoResponse> {
        return this.http.delete<ControllerInfoResponse>(`/controllers/${id}`, null);
    }

    startForceFeedback(id: string, offset: number): Promise<void> {
        return this.http.put<void>(`/inputs/${id}/forcefeedback/${offset}`, null);
    }

    stopForceFeedback(id: string, offset: number): Promise<void> {
        return this.http.delete<void>(`/inputs/${id}/forcefeedback/${offset}`, null);
    }

    startInputDevice(id: string, method: InputMethod): Promise<void> {
        return this.http.put<void>(`/inputs/${id}/${method}/running`);
    }

    stopInputDevice(id: string, method: InputMethod): Promise<void> {
        return this.http.delete<void>(`/inputs/${id}/${method}/running`);
    }

    getHidGuardianInfo(id: string): Promise<HidGuardianInfo> {
        return this.http.get<HidGuardianInfo>(`/inputs/${id}/hidguardian`);
    }

    enableHidGuardian(id: string): Promise<void> {
        return this.http.put<void>(`/inputs/${id}/hidguardian`, null);
    }

    disableHidGuardian(id: string): Promise<void> {
        return this.http.delete<void>(`/inputs/${id}/hidguardian`, null);
    }

    getNotifiations(): Promise<Notification[]> {
        return this.http.get<Notification[]>(`/notifications`);
    }

    acknowledgeNotification(id: string): Promise<void> {
        return this.http.put<void>(`/notifications/${id}/acknowledge`);
    }
}

export const rest = new RestService(http);
