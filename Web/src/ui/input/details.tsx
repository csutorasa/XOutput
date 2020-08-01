import React, { Component } from "react";
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import CircularProgress from '@material-ui/core/CircularProgress';
import Paper from '@material-ui/core/Paper';
import Switch from '@material-ui/core/Switch';
import LinearProgress from '@material-ui/core/LinearProgress';
import { rest, InputDeviceDetails, InputDeviceConfig } from "../../communication/rest";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import { Styles } from "@material-ui/core/styles/withStyles";
import { WebSocketService } from '../../communication/websocket';
import { InputValues } from "../../communication/input";
import { MessageBase } from "../../communication/message";
import { Dpad } from "./dpad";
import { StyleGenerator, Styled } from "../../utils";

type ClassNames = 'header' | 'bar' | 'iconWrapper';

const styles: StyleGenerator<ClassNames> = () => ({
  header: {
    margin: '10px 0',
  },
  bar: {
    transition: 'none',
  },
  iconWrapper: {
    margin: 'auto',
    textAlign: 'center',
  }
});

export interface InputDetailsProps extends Styled<ClassNames> {
  id: string;
}

interface InputDetailsState {
  device: InputDeviceDetails;
  config: InputDeviceConfig;
  values: { [offset: string]: number };
  forceFeedbacks: number[];
  hidGuardianEnabled: boolean;
  hidGuardianAvailable: boolean;
}

class InputDetailsComponent extends Component<InputDetailsProps, InputDetailsState> {

  private websocket: WebSocketService = new WebSocketService();
  state: InputDetailsState = {
    device: null,
    config: null,
    values: {},
    forceFeedbacks: [],
    hidGuardianAvailable: false,
    hidGuardianEnabled: false,
  };

  componentDidMount() {
    this.getDetails();
    this.getConfig();
    rest.getHidGuardianInfo(this.props.id).then(info => {
      this.setState({
        hidGuardianAvailable: info.available,
        hidGuardianEnabled: info.active,
      })
    })
    this.websocket.connect(`input/${this.props.id}`, (data: MessageBase) => {
      if (data.Type === 'InputValues') {
        const inputValues = data as InputValues;
        const newValues: { [offset: string]: number } = {};
        for (const offset in this.state.values) {
          newValues[offset] = this.state.values[offset];
        }
        for (const offset in inputValues.Values) {
          newValues[offset] = inputValues.Values[offset] * 100;
        }
        this.setState({
          values: newValues,
        });
      }
    });
  }

  changeConfig(add: boolean, config: string, offset: number) {
    let promise;
    if (add) {
      promise = rest.addInputDeviceConfig(this.props.id, config, offset);
    } else {
      promise = rest.removeInputDeviceConfig(this.props.id, config, offset);
    }
    return promise.then(() => this.getConfig());
  }

  getDetails() {
    return rest.getInputDeviceDetails(this.props.id).then(device => {
      this.setState({
        device: device
      })
    })
  }

  getConfig() {
    return rest.getInputDeviceConfig(this.props.id).then(config => {
      this.setState({
        config: config
      })
    })
  }

  switchForceFeedback(offset: number) {
    const enabled = this.state.forceFeedbacks.indexOf(offset) >= 0;
    let values: number[];
    if (enabled) {
      values = this.state.forceFeedbacks.filter(f => f != offset);
      rest.stopForceFeedback(this.props.id, offset);
    } else {
      values = this.state.forceFeedbacks.slice();
      values.push(offset);
      rest.startForceFeedback(this.props.id, offset);
    }
    this.setState({
      forceFeedbacks: values,
    })
  }

  dpadGroups(): { up?: number; down?: number; left?: number; right?: number }[] {
    const dpads: { up?: number; down?: number; left?: number; right?: number }[] = [];
    this.state.device.sources.filter(s => s.type == 'dpad').forEach(s => {
      const index = Math.floor((s.offset - 100000) / 4);
      if (!dpads[index]) {
        dpads[index] = {};
      }
      const value = dpads[index];
      switch (s.offset % 4) {
        case 0:
          value.up = this.state.values[s.offset] || 0;
          break;
        case 1:
          value.down = this.state.values[s.offset] || 0;
          break;
        case 2:
          value.left = this.state.values[s.offset] || 0;
          break;
        case 3:
          value.right = this.state.values[s.offset] || 0;
          break;
      }
    });
    return dpads;
  }

  hidGuardianChange(checked: boolean) {
    const promise = checked ? rest.enableHidGuardian(this.props.id) : rest.disableHidGuardian(this.props.id);
    promise.then(() => {
      return rest.getHidGuardianInfo(this.props.id).then(info => {
        this.setState({
          hidGuardianAvailable: info.available,
          hidGuardianEnabled: info.active,
        })
      })
    });
  }

  createForceFeedback(offset: number) {
    const active = this.state.forceFeedbacks.indexOf(offset) >= 0;
    const big = this.state.config ? this.state.config.bigMotors.indexOf(offset) >= 0 : false;
    const small = this.state.config ? this.state.config.smallMotors.indexOf(offset) >= 0 : false;
    return (<>
      <div>
        <Switch color='primary' checked={active} onChange={() => this.switchForceFeedback(offset)} />
        {Translation.translate("Test")}
      </div>
      <div>
        <Switch color='primary' checked={big} onChange={() => this.changeConfig((event.target as any).checked, "big", offset)} />
        {Translation.translate("BigMotor")}
      </div>
      <div>
        <Switch color='primary' checked={small} onChange={() => this.changeConfig((event.target as any).checked, "small", offset)} />
        {Translation.translate("SmallMotor")}
      </div>
    </>);
  }

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.device) {
      content = <CircularProgress />;
    } else {
      const dpads = this.dpadGroups().length == 0 ? <></> : (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("DPads")}</Typography>
        <Grid container spacing={3}>
          {this.dpadGroups().map((d, i) => <Grid item xs={6} md={4} lg={3} key={i}>
            <Paper>
              <Typography align='center' variant='body1'>{Translation.translate('DPad') + " " + (i + 1)}</Typography>
              <Dpad up={d.up} down={d.down} left={d.left} right={d.right} />
            </Paper>
          </Grid>)}
        </Grid>
      </>);
      const axes = this.state.device.sources.filter(s => s.type == 'axis').length == 0 ? <></> : (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Axes")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'axis').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0.5} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
      </>);
      const buttons = this.state.device.sources.filter(s => s.type == 'button').length == 0 ? <></> : (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Buttons")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'button').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
      </>);
      const sliders = this.state.device.sources.filter(s => s.type == 'slider').length == 0 ? <></> : (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Sliders")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'slider').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
      </>);
      const forceFeedbacks = this.state.device.forceFeedbacks.length == 0 ? <></> : (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("ForceFeedbacks")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.forceFeedbacks.map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper>
              <Typography align='center' variant='body1'>{s.offset}</Typography>
              {this.createForceFeedback(s.offset)}
            </Paper>
          </Grid>)}
        </Grid>
      </>);
      let hidguardian;
      if (this.state.hidGuardianAvailable) {
        hidguardian = (<>
          <Typography className={classes.header} variant='h5'>{Translation.translate("HidGuardian")}</Typography>
          <Paper>
            <Typography className={classes.header} variant='body1'>{this.state.device.hardwareId}</Typography>
            <Switch color='primary' checked={this.state.hidGuardianEnabled} onChange={() => this.hidGuardianChange((event.target as any).checked)} />
          </Paper>
        </>);
      } else {
        hidguardian = <></>
      }
      content = (<>
        {dpads}
        {axes}
        {buttons}
        {sliders}
        {forceFeedbacks}
        {hidguardian}
      </>);
    }
    return <>
      <Typography variant='h3'>{Translation.translate("InputDevices")}</Typography>
      {content}
    </>;
  }
}

export const InputDetails = withStyles(styles)(InputDetailsComponent);
