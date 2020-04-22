import React from "react";
import Grid from '@material-ui/core/Grid';
import Chip from '@material-ui/core/Chip';
import Tooltip from '@material-ui/core/Tooltip';
import Typography from '@material-ui/core/Typography';
import CircularProgress from '@material-ui/core/CircularProgress';
import Paper from '@material-ui/core/Paper';
import MouseIcon from '@material-ui/icons/Mouse';
import KeyboardIcon from '@material-ui/icons/Keyboard';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import ZoomOutMapIcon from '@material-ui/icons/ZoomOutMap';
import GamepadIcon from '@material-ui/icons/Gamepad';
import RadioButtonCheckedIcon from '@material-ui/icons/RadioButtonChecked';
import RadioButtonUncheckedIcon from '@material-ui/icons/RadioButtonUnchecked';
import { rest, InputDeviceInfoResponse, InputDeviceInformation } from "../../communication/rest";
import { Link } from "react-router-dom";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import { Styles } from "@material-ui/core/styles/withStyles";

interface InputsState {
  devices: InputDeviceInfoResponse;
}

const styles: Styles<Theme, any, any> = () => ({
  paper: {
    height: '100%',
    width: '100%',
  },
  chip: {
    margin: '5px 5px',
  },
  iconWrapper: {
    margin: 'auto',
    textAlign: 'center',
  }
});

export class InputsComponent extends React.Component<any, InputsState, any> {

  constructor(props: Readonly<any>) {
    super(props);
    this.state = {
      devices: null,
    };
  }

  componentDidMount() {
    this.refreshDevices();
  }

  refreshDevices() {
    return rest.getInputDevices().then(devices => {
      this.setState({
        devices: devices,
      })
    })
  }

  private deviceToIcon(device: InputDeviceInformation) {
    if (device.name == 'Keyboard') {
      return <KeyboardIcon />;
    }
    if (device.name == 'Mouse') {
      return <MouseIcon />;
    }
    return <SportsEsportsIcon />;
  }

  private translateName(name: string): string {
    if (name == 'mouse') {
      return Translation.translate('Mouse');
    }
    if (name == 'keyboard') {
      return Translation.translate('Keyboard');
    }
    return name;
  }

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.devices) {
      content = <CircularProgress />;
    } else {
      content = (<Grid container spacing={4}>
        {this.state.devices.map(d => <Grid item xs={12} md={6} lg={4} key={d.id}>
          <Paper className={classes.paper}>
            <Grid container>
              <Grid item xs={1} className={classes.iconWrapper}>
                {this.deviceToIcon(d)}
              </Grid>
              <Grid item xs={11}>
                <Typography variant='body1'><Link to={`/inputs/${d.id}`}>{this.translateName(d.name)}</Link></Typography>
                <Typography variant='body2'>{d.id}</Typography>
              </Grid>
            </Grid>
            <div>
              <Tooltip title={Translation.translate('DPads')}>
                <Chip variant='outlined' className={classes.chip} icon={<GamepadIcon />} label={d.dPads} />
              </Tooltip>
              <Tooltip title={Translation.translate('Axes')}>
                <Chip variant='outlined' className={classes.chip} icon={<ZoomOutMapIcon />} label={d.axes} />
              </Tooltip>
              <Tooltip title={Translation.translate('Buttons')}>
                <Chip variant='outlined' className={classes.chip} icon={<RadioButtonUncheckedIcon />} label={d.buttons} />
              </Tooltip>
              <Tooltip title={Translation.translate('Sliders')}>
                <Chip variant='outlined' className={classes.chip} icon={<RadioButtonCheckedIcon />} label={d.sliders} />
              </Tooltip>
            </div>
          </Paper>
        </Grid>)}
      </Grid>);
    }
    return <>
      <h1>{Translation.translate("InputDevices")}</h1>
      {content}
    </>;
  }
}

export const InputsPage = withStyles(styles)(InputsComponent);
