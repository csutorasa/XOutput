import { Http, HttpService } from "./http";

export interface EmulatorResponse {
    Installed: boolean;
    SupportedDeviceTypes: string[];
}

export interface ListEmulatorsResponse {
    [emulator: string]: EmulatorResponse;
}

class RestService {
    private http: HttpService;

    constructor(http: HttpService) {
        this.http = http;
    }

    getEmulators(): Promise<ListEmulatorsResponse> {
        return this.http.get<ListEmulatorsResponse>('/emulators');
    }
}

export const Rest = new RestService(Http);
