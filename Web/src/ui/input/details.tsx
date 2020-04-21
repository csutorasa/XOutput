import React from "react";
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import CircularProgress from '@material-ui/core/CircularProgress';
import Paper from '@material-ui/core/Paper';
import LinearProgress from '@material-ui/core/LinearProgress';
import { rest, InputDeviceDetails } from "../../communication/rest";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import { Styles } from "@material-ui/core/styles/withStyles";
import { WebSocketService } from '../../communication/websocket';
import { InputValues } from "../../communication/input";
import { MessageBase } from "../../communication/message";
import { DpadComponent } from "./dpad";

interface InputDetailsState {
  device: InputDeviceDetails;
  values: { [offset: string]: number };
  forceFeedbacks: number[];
}

export interface InputDetailsProps {
  id: string;
  classes?: any;
}

const styles: Styles<Theme, any, any> = () => ({
  paper: {
    height: '100%',
    width: '100%',
  },
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

export class InputDetailsComponent extends React.Component<InputDetailsProps, InputDetailsState, any> {

  private websocket: WebSocketService = new WebSocketService();

  constructor(props: Readonly<any>) {
    super(props);
    this.state = {
      device: null,
      values: {},
      forceFeedbacks: [],
    };
  }

  componentDidMount() {
    this.refreshDevices();
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

  refreshDevices() {
    return rest.getInputDeviceDetails(this.props.id).then(device => {
      this.setState({
        device: device
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
    this.state.device.sources.filter(s => s.type == 'dpad').forEach(s =>{
      const index = Math.floor((s.offset - 10000) / 4);
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

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.device) {
      content = <CircularProgress />;
    } else {
      content = (<>
        <Typography className={classes.header} variant='h5'>{Translation.translate("DPads")}</Typography>
        <Grid container spacing={3}>
          {this.dpadGroups().map((d, i) => <Grid item xs={6} md={4} lg={3} key={i}>
            <Paper className={classes.paper}>
              <Typography align='center' variant='body1'>{Translation.translate('DPad') + " " + (i + 1)}</Typography>
              <DpadComponent up={d.up} down={d.down} left={d.left} right={d.right} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Axes")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'axis').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0.5} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Buttons")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'button').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography className={classes.header} variant='h5'>{Translation.translate("Sliders")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.sources.filter(s => s.type == 'slider').map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography align='center' variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography className={classes.header} variant='h5'>{Translation.translate("ForceFeedbacks")}</Typography>
        <Grid container spacing={3}>
          {this.state.device.forceFeedbacks.map(s => <Grid item xs={6} md={4} lg={3} key={s.offset}>
            <Paper className={classes.paper} onClick={() => this.switchForceFeedback(s.offset)}>
              <Typography align='center' variant='body1'>{s.offset}</Typography>
              <LinearProgress variant={this.state.forceFeedbacks.indexOf(s.offset) >= 0 ? "indeterminate" : "determinate"} />
            </Paper>
          </Grid>)}
        </Grid>
      </>);
    }
    return <>
      <Typography variant='h3'>{Translation.translate("InputDevices")}</Typography>
      {content}
    </>;
  }
}

export const InputDetailsPage = withStyles(styles)(InputDetailsComponent);
