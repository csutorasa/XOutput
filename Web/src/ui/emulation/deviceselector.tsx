import React, { ReactElement, Fragment } from "react";
import { ListEmulatorsResponse, rest, EmulatorResponse } from "../../communication/rest";
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import { Translation } from "../../translation/translation";
import { Link } from "react-router-dom";
import { withStyles } from "@material-ui/core";
import Async from "../components/Asnyc";
import { Styled, StyleGenerator } from "../../utils";
import QRCode from 'qrcode';

type ClassNames = 'deviceSeparator';

const styles: StyleGenerator<ClassNames> = () => ({
    deviceSeparator: {
      height: '15px',
    },
});

export interface DeviceSelectorProps extends Styled<ClassNames> {

}

interface DeviceSelectorState {
    emulators: ListEmulatorsResponse;
}

export class DeviceSelectorPage extends React.Component<DeviceSelectorProps, DeviceSelectorState> {

    state: DeviceSelectorState = {
        emulators: null,
    };

    private loading: Promise<void>;
    private qrGeneration: Promise<string | void>;
    private path = window.location.toString();

    componentDidMount() {
        this.loading = rest.getEmulators().then(r => {
            this.setState({
                emulators: r,
            });
        });
        this.qrGeneration = QRCode.toDataURL(this.path, { type: 'image/jpeg' });
        this.forceUpdate();
    }

    private renderButton(emulator: string, device: string, installed: boolean): ReactElement {
        const { classes } = this.props;
        const element = <Fragment key={`${device}/${emulator}`}>
            <div className={classes.deviceSeparator}></div>
            <Button variant="contained" color='primary' disabled={!installed}>
                {Translation.translate(device)}
            </Button>
            <div className={classes.deviceSeparator}></div>
        </Fragment>;
        if (installed) {
            return <Link key={`${device}/${emulator}`} to={`/emulation/${device}/${emulator}`}>
                {element}
            </Link>
        }
        return element;
    }

    private renderListElement(key: string, value: EmulatorResponse): ReactElement {
        return <div key={key}>
            <Typography variant='h5'>{key}</Typography>
            {value.supportedDeviceTypes.map(t => (this.renderButton(key, t, value.installed)))}
        </div>;
    }

    render() {
        return (<>
            <Typography variant='h3'>{Translation.translate("OnlineDevices")}</Typography>
            <Async task={this.loading}>
            { () => Object.keys(this.state.emulators).map(key => this.renderListElement(key, this.state.emulators[key])) }
            </Async>
            <Typography variant='h5'>{Translation.translate("Share")}</Typography>
            <Async task={this.qrGeneration}>
                { (dataUrl: string) => <>
                    <Typography variant='body1'>{this.path}</Typography>
                    <img src={dataUrl} />
                    </>
                }
            </Async>
        </>);
    }
}

export const DeviceSelector = withStyles(styles)(DeviceSelectorPage);
export default DeviceSelector;
