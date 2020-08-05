import React, { Component } from "react";
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import CircularProgress from '@material-ui/core/CircularProgress';
import Accordion from '@material-ui/core/Accordion';
import Paper from '@material-ui/core/Paper';
import Switch from '@material-ui/core/Switch';
import LinearProgress from '@material-ui/core/LinearProgress';
import { rest, InputDeviceDetails, InputDeviceConfig, InputDeviceInputDetails, InputMethod } from "../../communication/rest";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import { WebSocketService, WebSocketSession } from '../../communication/websocket';
import { InputValues } from "../../communication/input";
import { MessageBase } from "../../communication/message";
import { Dpad } from "./dpad";
import { StyleGenerator, Styled } from "../../utils";

type ClassNames = 'header' | 'paper' | 'bar' | 'iconWrapper';

const styles: StyleGenerator<ClassNames> = () => ({
  header: {
    margin: '10px 0',
  },
  paper: {
    padding: '10px',
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
  values: { [method: string]: { [offset: string]: number } };
  forceFeedbacks: number[];
  hidGuardianEnabled: boolean;
  hidGuardianAvailable: boolean;
}

class InputDetailsComponent extends Component<InputDetailsProps, InputDetailsState> {

  private websocket: WebSocketService = new WebSocketService();
  private websocketSession: WebSocketSession;
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
        const newValues: { [method: string]: { [offset: string]: number } } = Object.assign(this.state.values, {});
        for (const index in inputValues.Values) {
          const method =  inputValues.Values[index].Method;
          const offset =  inputValues.Values[index].Offset;
          if (!newValues[method]) {
            newValues[method] = {};
          }
          newValues[method][offset] = inputValues.Values[index].Value * 100;
        }
        this.setState({
          values: newValues,
        });
      }
    }).then((s) => this.websocketSession = s);
  }

  componentWillUnmount() {
    this.websocketSession?.close();
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

  getValue(method: InputMethod, offset: number, defaultValue: number = 0): number {
    if (!this.state.values) {
      return null;
    }
    if (!this.state.values[method]) {
      this.state.values[method] = {};
      return null;
    }
    if (!this.state.values[method][offset]) {
      this.state.values[method][offset] = defaultValue;
    }
    return this.state.values[method][offset];
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

  dpadGroups(inputDetails: InputDeviceInputDetails): { up?: number; down?: number; left?: number; right?: number }[] {
    const dpads: { up?: number; down?: number; left?: number; right?: number }[] = [];
    inputDetails.sources.filter(s => s.type == 'dpad').forEach(s => {
      const index = Math.floor((s.offset - 100000) / 4);
      if (!dpads[index]) {
        dpads[index] = {};
      }
      const value = dpads[index];
      switch (s.offset % 4) {
        case 0:
          value.up = this.getValue(inputDetails.inputMethod, s.offset);
          break;
        case 1:
          value.down = this.getValue(inputDetails.inputMethod, s.offset)
          break;
        case 2:
          value.left = this.getValue(inputDetails.inputMethod, s.offset);
          break;
        case 3:
          value.right = this.getValue(inputDetails.inputMethod, s.offset);
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

  runningChange(checked: boolean, method: InputMethod) {
    const promise = checked ? rest.startInputDevice(this.props.id, method) : rest.stopInputDevice(this.props.id, method);
    promise.then(() => this.getDetails());
  }

  createForceFeedback(offset: number) {
    const active = this.state.forceFeedbacks.indexOf(offset) >= 0;
    return (<>
      <div>
        <Switch color='primary' checked={active} onChange={() => this.switchForceFeedback(offset)} />
        {Translation.translate("Test")}
      </div>
    </>);
  }

  getDPads(inputDetails: InputDeviceInputDetails) {
    const groups = this.dpadGroups(inputDetails);
    if (groups.length == 0) {
      return null;
    }
    const { classes } = this.props;
    return <>
      <Typography className={classes.header} variant='h5'>{Translation.translate("DPads")}</Typography>
      <Grid container spacing={3}>
        {groups.map((d, i) => <Grid item xs={6} md={4} lg={3} key={i}>
          <Paper>
            <Typography align='center' variant='body1'>{Translation.translate('DPad') + " " + (i + 1)}</Typography>
            <Dpad up={d.up} down={d.down} left={d.left} right={d.right} />
          </Paper>
        </Grid>)}
      </Grid>
    </>;
  }

  getButtons(inputDetails: InputDeviceInputDetails) {
    if (inputDetails.sources.filter(s => s.type == 'button').length == 0) {
      return null;
    }
    const { classes } = this.props;
    return <>
      <Typography className={classes.header} variant='h5'>{Translation.translate("Buttons")}</Typography>
      <Grid container spacing={3}>
        {inputDetails.sources.filter(s => s.type == 'button').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
          <Paper>
            <Typography align='center' variant='body1'>{s.name}</Typography>
            <LinearProgress variant="determinate" value={this.getValue(inputDetails.inputMethod, s.offset)} classes={{ bar: classes.bar }} />
          </Paper>
        </Grid>)}
      </Grid>
    </>;
  }

  getAxes(inputDetails: InputDeviceInputDetails) {
    if (inputDetails.sources.filter(s => s.type == 'axis').length == 0) {
      return null;
    }
    const { classes } = this.props;
    return <>
      <Typography className={classes.header} variant='h5'>{Translation.translate("Axes")}</Typography>
      <Grid container spacing={3}>
        {inputDetails.sources.filter(s => s.type == 'axis').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
          <Paper>
            <Typography align='center' variant='body1'>{s.name}</Typography>
            <LinearProgress variant="determinate" value={this.getValue(inputDetails.inputMethod, s.offset, 0.5)} classes={{ bar: classes.bar }} />
          </Paper>
        </Grid>)}
      </Grid>
    </>;
  }

  getSliders(inputDetails: InputDeviceInputDetails) {
    if (inputDetails.sources.filter(s => s.type == 'slider').length == 0) {
      return null;
    }
    const { classes } = this.props;
    return <>
      <Typography className={classes.header} variant='h5'>{Translation.translate("Sliders")}</Typography>
      <Grid container spacing={3}>
        {inputDetails.sources.filter(s => s.type == 'slider').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
          <Paper>
            <Typography align='center' variant='body1'>{s.name}</Typography>
            <LinearProgress variant="determinate" value={this.getValue(inputDetails.inputMethod, s.offset)} classes={{ bar: classes.bar }} />
          </Paper>
        </Grid>)}
      </Grid>
    </>;
  }

  getForceFeedbacks(inputDetails: InputDeviceInputDetails) {
    if (inputDetails.forceFeedbacks.length == 0) {
      return null;
    }
    const { classes } = this.props;
    return <>
      <Typography className={classes.header} variant='h5'>{Translation.translate("ForceFeedbacks")}</Typography>
      <Grid container spacing={3}>
        {inputDetails.forceFeedbacks.map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
          <Paper>
            <Typography align='center' variant='body1'>{s.offset}</Typography>
            {this.createForceFeedback(s.offset)}
          </Paper>
        </Grid>)}
      </Grid>
    </>;
  }

  mapMethod(method: InputMethod) {
    switch(method) {
      case InputMethod.WindowsApi:
        return 'Windows API';
      case InputMethod.DirectInput:
        return 'Direct Input';
      case InputMethod.RawInput:
        return 'RawInput';
      case InputMethod.Websocket:
        return 'Web device';
      default:
        return method;
    }
  }

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.device) {
      content = <CircularProgress />;
    } else {
      const summary = <Paper className={classes.paper}>
        <p>{ Translation.translate('input.name') }: { this.state.device.name }</p>
        <p>{ Translation.translate('input.id') }: { this.state.device.id }</p>
        <p>{ Translation.translate('input.hardwareid') }: { this.state.device.hardwareId }</p>
        { this.state.hidGuardianAvailable ?
            <div>
              <span>{ Translation.translate('input.hidguardian') }</span>
              <Switch color='primary'
                checked={this.state.hidGuardianEnabled}
                onChange={() => this.hidGuardianChange((event.target as any).checked)} />
            </div>
            : null
        }
      </Paper>;

      content = (<>
        {summary}
        {this.state.device.inputs.map(i => <div className={classes.paper} key={i.inputMethod}>
          <Typography className={classes.header} variant='h4'>{this.mapMethod(i.inputMethod)}</Typography>
          <Switch color='primary'
                checked={i.running}
                onChange={() => this.runningChange((event.target as any).checked, i.inputMethod)} />
          <div style={{width: '100%'}}>
            { this.getDPads(i) }
            { this.getAxes(i) }
            { this.getButtons(i) }
            { this.getSliders(i) }
            { this.getForceFeedbacks(i) }
          </div>
        </div>)}
      </>);
    }
    return <>
      <Typography variant='h3'>{Translation.translate("InputDevices")}</Typography>
      {content}
    </>;
  }
}

export const InputDetails = withStyles(styles)(InputDetailsComponent);
