import React, { ReactElement } from "react";
import { ListEmulatorsResponse, rest, EmulatorResponse } from "../../communication/rest";
import CircularProgress from '@material-ui/core/CircularProgress';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import { Translation } from "../../translation/Translation";
import { Link } from "react-router-dom";
import { Styles } from "@material-ui/core/styles/withStyles";
import { Theme, withStyles } from "@material-ui/core";

interface DeviceSelectorState {
    emulators: ListEmulatorsResponse;
    loading: boolean;
}

const styles: Styles<Theme, any, any> = () => ({
    deviceSeparator: {
      height: '15px',
    },
});

export class DeviceSelectorPage extends React.Component<any, DeviceSelectorState, any> {

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

        if (this.state.loading) {
            return <CircularProgress />;
        }
        return <>
            <Typography variant='h3'>{Translation.translate("OnlineDevices")}</Typography>
            {Object.keys(this.state.emulators).map(key => this.renderListElement(key, this.state.emulators[key]))}
        </>;
    }
}

export const DeviceSelector = withStyles(styles)(DeviceSelectorPage);
