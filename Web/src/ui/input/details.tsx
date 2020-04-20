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

interface InputDetailsState {
  device: InputDeviceDetails;
  values: { [offset: string]: number };
}

export interface InputDetailsProps {
  id: string;
  classes?: any;
}

const styles: Styles<Theme, any, any> = () => ({
  container: {
    margin: '10px 0',
  },
  paper: {
    height: '100%',
    width: '100%',
    padding: '5px',
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

  render() {
    const { classes } = this.props;

    let content;
    if (!this.state.device) {
      content = <CircularProgress />;
    } else {
      content = (<>
        <Typography variant='h5'>{Translation.translate("DPads")}</Typography>
        <Grid container className={classes.container} spacing={2}>
          {this.state.device.sources.filter(s => s.type == 'dpad').map(s => <Grid item xs={12} md={6} lg={4} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography variant='h5'>{Translation.translate("Axes")}</Typography>
        <Grid container className={classes.container} spacing={2}>
          {this.state.device.sources.filter(s => s.type == 'axis').map(s => <Grid item xs={12} md={6} lg={4} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0.5} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography variant='h5'>{Translation.translate("Buttons")}</Typography>
        <Grid container className={classes.container} spacing={2}>
          {this.state.device.sources.filter(s => s.type == 'button').map(s => <Grid item xs={12} md={6} lg={4} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography variant='h5'>{Translation.translate("Sliders")}</Typography>
        <Grid container className={classes.container} spacing={2}>
          {this.state.device.sources.filter(s => s.type == 'slider').map(s => <Grid item xs={12} md={6} lg={4} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography variant='body1'>{s.name}</Typography>
              <LinearProgress variant="determinate" value={this.state.values[s.offset] || 0} classes={{ bar: classes.bar }} />
            </Paper>
          </Grid>)}
        </Grid>
        <Typography variant='h5'>{Translation.translate("ForceFeedbacks")}</Typography>
        <Grid container className={classes.container} spacing={2}>
          {this.state.device.forceFeedbacks.map(s => <Grid item xs={12} md={6} lg={4} key={s.offset}>
            <Paper className={classes.paper}>
              <Typography variant='body1'>{s.offset}</Typography>
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
