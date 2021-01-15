import React, { Component } from "react";
import { green, blue, grey, red } from '@material-ui/core/colors';
import Grid from '@material-ui/core/Grid';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import Tooltip from '@material-ui/core/Tooltip';
import Paper from '@material-ui/core/Paper';
import SportsEsportsIcon from '@material-ui/icons/SportsEsports';
import AddCircleOutline from '@material-ui/icons/AddCircleOutline';
import DeleteIcon from '@material-ui/icons/Delete';
import { ControllerInfoResponse, DeviceType, rest, ControllerInfo } from "../../communication/rest";
import { Translation } from "../../translation/translation";
import { withStyles, Theme } from "@material-ui/core";
import Async from "../components/Asnyc";
import { StyleGenerator, Styled } from "../../utils";

type ClassNames = never;

const styles: StyleGenerator<ClassNames> = () => ({

});

export interface HomeProps extends Styled<ClassNames> {

}

interface HomeState {

}

class HomeComponent extends Component<HomeProps, HomeState> {
  state: HomeState = {

  };

  componentDidMount() {
  }

  render() {
    const { classes } = this.props;
    return <></>;
  }
}

export const Home = withStyles(styles)(HomeComponent);
