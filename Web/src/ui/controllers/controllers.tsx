import React from "react";
import List from '@material-ui/core/List';
import { green, blue, grey, red } from '@material-ui/core/colors';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import Tooltip from '@material-ui/core/Tooltip';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import { DeviceInfoResponse, DeviceType, rest, DeviceInfo } from "../../communication/rest";
import { Translation } from "../../translation/translation";

interface ControllersState {
    devices: DeviceInfoResponse;
}

export class ControllersPage extends React.Component<any, ControllersState, any> {

    constructor(props: Readonly<any>) {
        super(props);
        this.state = {
          devices: null
        };
    }

    componentDidMount() {
      rest.getDevices().then(devices => {
        this.setState({
          devices: devices
        })
      })
    }

    deviceInfoToColor(deviceInfo: DeviceInfo): string {
      if (!deviceInfo.active) {
        return grey[500];
      }
      switch (deviceInfo.deviceType) {
        case DeviceType.MicrosoftXbox360:
          return green[500];
        case DeviceType.SonyDualShock4:
          return blue[500];
        default:
          return red[500];
      }
    }

    render() {
      let content;
      if (!this.state.devices) {
        content = <h1>Loading</h1>
      } else {
        content = (this.state.devices.map(d => <List>
          <ListItem>
            <ListItemIcon>
              <Tooltip title={ Translation.translate(d.deviceType) + ' - ' + d.emulator }>
                <SportsEsportsIcon style={{ color: this.deviceInfoToColor(d) }} />
              </Tooltip>
            </ListItemIcon>
            <ListItemText primary={ d.id } secondary={ d.address } />
          </ListItem>
        </List>));
      }
      return <>
        <h1>Controllers</h1>
        { content }
      </>;
    }
}
