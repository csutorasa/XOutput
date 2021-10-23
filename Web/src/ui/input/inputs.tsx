import React, { Component } from 'react';
import Grid from '@material-ui/core/Grid';
import Chip from '@material-ui/core/Chip';
import Tooltip from '@material-ui/core/Tooltip';
import Typography from '@material-ui/core/Typography';
import Paper from '@material-ui/core/Paper';
import MouseIcon from '@material-ui/icons/Mouse';
import KeyboardIcon from '@material-ui/icons/Keyboard';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import ZoomOutMapIcon from '@material-ui/icons/ZoomOutMap';
import GamepadIcon from '@material-ui/icons/Gamepad';
import RadioButtonCheckedIcon from '@material-ui/icons/RadioButtonChecked';
import RadioButtonUncheckedIcon from '@material-ui/icons/RadioButtonUnchecked';
import { rest, InputDeviceInfoResponse, InputDeviceInformation } from '../../client/rest';
import { Link } from 'react-router-dom';
import { Translation } from '../../translation/Translation';
import { withStyles, Theme } from '@material-ui/core';
import { StyleGenerator, Styled } from '../../utils';
import { Async } from '../components/Asnyc';

type ClassNames = 'paper' | 'chip' | 'iconWrapper';

interface InputsState {
  devices: InputDeviceInfoResponse;
}

const styles: StyleGenerator<ClassNames> = () => ({
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
  },
});

export interface InputsProps extends Styled<ClassNames> {}

class InputsComponent extends Component<InputsProps, InputsState> {
  state: InputsState = {
    devices: null,
  };

  private loading: Promise<void>;

  componentDidMount() {
    this.loading = this.refreshDevices();
    this.forceUpdate();
  }

  refreshDevices() {
    return rest.getInputDevices().then((devices) => {
      this.setState({
        devices,
      });
    });
  }

  private deviceToIcon(device: InputDeviceInformation) {
    if (device.id === 'keyboard') {
      return <KeyboardIcon />;
    }
    if (device.id === 'mouse') {
      return <MouseIcon />;
    }
    return <SportsEsportsIcon />;
  }

  private translateName(name: string): string {
    if (name === 'mouse') {
      return Translation.translate('Mouse');
    }
    if (name === 'keyboard') {
      return Translation.translate('Keyboard');
    }
    return name;
  }

  private mapFeatureToChip(feature: string) {
    const { classes } = this.props;
    let label;
    switch (feature) {
      case 'WindowsApi':
        label = 'Windows API';
        break;
      case 'RawInput':
        label = 'Raw Input';
        break;
      case 'DirectInput':
        label = 'Direct Input';
        break;
    }
    return <Chip variant="outlined" className={classes.chip} label={label} key={feature} />;
  }

  render() {
    const { classes } = this.props;
    return (
      <>
        <Typography variant="h3">{Translation.translate('InputDevices')}</Typography>
        <Async task={this.loading}>
          {() => (
            <Grid container spacing={2}>
              {this.state.devices.map((d) => (
                <Grid item xs={12} md={6} lg={4} key={d.id}>
                  <Paper className={classes.paper}>
                    <Grid container>
                      <Grid item xs={1} className={classes.iconWrapper}>
                        {this.deviceToIcon(d)}
                      </Grid>
                      <Grid item xs={11}>
                        <Typography variant="body1">
                          <Link to={`/inputs/${d.id}`}>{this.translateName(d.name)}</Link>
                        </Typography>
                        <Typography variant="body2">{d.id}</Typography>
                      </Grid>
                    </Grid>
                    <div>{d.activeFeatures.map((f) => this.mapFeatureToChip(f))}</div>
                  </Paper>
                </Grid>
              ))}
            </Grid>
          )}
        </Async>
      </>
    );
  }
}

export const Inputs = withStyles(styles)(InputsComponent);
