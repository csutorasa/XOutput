import React, { ReactElement } from "react";
import { ListEmulatorsResponse, rest, EmulatorResponse } from "../../communication/rest";
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import { Translation } from "../../translation/Translation";
import { Link } from "react-router-dom";
import { Styles } from "@material-ui/core/styles/withStyles";
import { Theme, withStyles } from "@material-ui/core";
import { Async, AsyncErrorHandler } from "../components/Asnyc";

type ClassNames = 'deviceSeparator';

const styles: Styles<Theme, any, any> = () => ({
    deviceSeparator: {
      height: '15px',
    },
});

interface DeviceSelectorState {
    emulators: ListEmulatorsResponse;
}

export class DeviceSelectorPage extends React.Component<any, DeviceSelectorState> {
    
    state: DeviceSelectorState = {
        emulators: null,
    };

    private loading: Promise<void>;

    componentDidMount() {
        this.loading = rest.getEmulators().then(r => {
            this.setState({
                emulators: r,
            });
        }, AsyncErrorHandler(this));
    }

    private renderButton(emulator: string, device: string, installed: boolean): ReactElement {
        const { classes } = this.props;
        const element = <>
            <div className={classes.deviceSeparator}></div>
            <Button variant="contained" color='primary' key={`${device}/${emulator}`} disabled={!installed}>
                {Translation.translate(device)}
            </Button>
            <div className={classes.deviceSeparator}></div>
        </>;
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
        return (<Async task={this.loading} render={() => <>
            <Typography variant='h3'>{Translation.translate("OnlineDevices")}</Typography>
            { Object.keys(this.state.emulators).map(key => this.renderListElement(key, this.state.emulators[key]))}
            </>}
        />);
    }
}

export const DeviceSelector = withStyles(styles)(DeviceSelectorPage);
