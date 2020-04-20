import React from "react";
import { green, blue, grey, red } from '@material-ui/core/colors';
import Grid from '@material-ui/core/Grid';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import CircularProgress from '@material-ui/core/CircularProgress';
import Tooltip from '@material-ui/core/Tooltip';
import Paper from '@material-ui/core/Paper';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import DeleteIcon from '@material-ui/icons/Delete';
import { ControllerInfoResponse, DeviceType, rest, ControllerInfo } from "../../communication/rest";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import { Styles } from "@material-ui/core/styles/withStyles";

interface ControllersState {
  devices: ControllerInfoResponse;
}

const styles: Styles<Theme, any, any> = () => ({
    container: {
      margin: '10px 0',
    },
    paper: {
      height: '100%',
      width: '100%',
    },
    iconWrapper: {
      margin: 'auto',
      textAlign: 'center',
    }
});

export class ControllersComponent extends React.Component<any, ControllersState, any> {

  constructor(props: Readonly<any>) {
    super(props);
    this.state = {
      devices: null
    };
  }


  componentDidMount() {
    this.refreshDevices();
  }

  refreshDevices() {
    return rest.getControllers().then(devices => {
      this.setState({
        devices: devices
      })
    })
  }

  deviceInfoToColor(deviceInfo: ControllerInfo): string {
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

  deleteDevice(id: string) {
    rest.removeControllers(id).then(() => this.refreshDevices());
  }

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.devices) {
      content = <CircularProgress />;
    } else {
      content = (<Grid container className={classes.container} spacing={2}>
          {this.state.devices.map(d => <Grid item xs={12} md={6} lg={4} key={d.id}>
          <Paper className={classes.paper}>
            <Grid container>
              <Grid item xs={1} className={classes.iconWrapper}>
                <Tooltip title={Translation.translate(d.deviceType) + ' - ' + d.emulator}>
                  <SportsEsportsIcon style={{ color: this.deviceInfoToColor(d) }}/>
                </Tooltip>
              </Grid>
              <Grid item xs={10}>
                <Typography variant='body1'>{Translation.translate(d.local ? 'LocalDevice' : 'WebDevice') + ` (${d.id})`}</Typography>
                <Typography variant='body2'>{d.local ? undefined : d.address}</Typography>
              </Grid>
              <Grid item xs={1} className={classes.iconWrapper}>
                <IconButton onClick={() => this.deleteDevice(d.id)}>
                  <DeleteIcon />
                </IconButton>
              </Grid>
            </Grid>
          </Paper>
        </Grid>)}
      </Grid>);
    }
    return <>
      <h1>{Translation.translate("ActiveControllers")}</h1>
      {content}
    </>;
  }
}
 
export const ControllersPage = withStyles(styles)(ControllersComponent);
