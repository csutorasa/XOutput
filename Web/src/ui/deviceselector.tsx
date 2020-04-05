import React, { ReactElement } from "react";
import { ListEmulatorsResponse, rest, EmulatorResponse } from "../communication/rest";
import { useHistory } from "react-router";
import { Link } from "react-router-dom";

interface DeviceSelectorState {
    emulators: ListEmulatorsResponse;
    loading: boolean;
}

export class DeviceSelector extends React.Component<any, DeviceSelectorState, any> {

    constructor(props: Readonly<any>) {
        super(props);
        this.state = {
            emulators: null,
            loading: true,
        }
        rest.getEmulators().then(r => {
            this.setState((state, props) => {
                return {
                    emulators: r,
                    loading: false,
                };
            });
        });
    }

    private renderButton(emulator: string, device: string, installed: boolean): ReactElement {
        const element = <button key={`${device}/${emulator}`} disabled={!installed}>
            {device}
        </button>;
        if (installed) {
            return <Link key={`${device}/${emulator}`} to={`/emulation/${device}/${emulator}`}>
                {element}
            </Link>
        }
        return element;
    }

    private renderListElement(key: string, value: EmulatorResponse): ReactElement {
        return <div key={key}>
            <h3>{key}</h3>
            <p>
                {value.supportedDeviceTypes.map(t => (this.renderButton(key, t, value.installed)))}
            </p>
        </div>;
    }

    render() {
        if (this.state.loading) {
            return <h1>Loading</h1>;
        }
        return <>
            {Object.keys(this.state.emulators).map(key => this.renderListElement(key, this.state.emulators[key]))}
        </>;
    }
}
