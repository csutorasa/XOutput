import React from 'react';
import { Switch, Route, Redirect, RouteChildrenProps, withRouter, RouteComponentProps } from 'react-router';
import { XboxEmulation } from './emulation/xbox';
import withStyles from '@mui/styles/withStyles';
import { Ds4Emulation } from './emulation/ds4';
import { Styled, StyleGenerator } from '../utils';
import { Notifications } from './notifications/Notifications';
import { MainMenu } from './MainMenu';
import { InputReader } from './input/InputReader';
import { Controllers } from './emulation/Controllers';

type ClassNames = 'mainContent' | 'title';

const styles: StyleGenerator<ClassNames> = (theme) => ({
  mainContent: {
    margin: '8px',
  },
  title: {
    flexGrow: 1,
    color: theme.palette.common.white,
  },
});

export type RouterProps = {};

type InternalRouterProps = Styled<ClassNames> & RouteComponentProps & RouterProps;

export interface MainMenuState {
  menuOpen: boolean;
  notificationCount: number;
}

const RouterComponent = ({ classes, match }: InternalRouterProps) => {
  return (
    <>
      <Switch>
        <Route path="/emulation" />
        <Route>
          <MainMenu />
        </Route>
      </Switch>
      <div className={classes.mainContent}>
        <Switch>
          <Route path="/" exact>
            <div></div>
          </Route>
          <Route path="/controllers" exact>
            <Controllers></Controllers>
          </Route>
          <Route path="/inputreader" exact>
            <InputReader></InputReader>
          </Route>
          <Route path="/notifications" exact>
            <Notifications></Notifications>
          </Route>
          <Route>
            <Redirect to="/" />
          </Route>
        </Switch>
      </div>
    </>
  );
};

export const Router = withRouter(withStyles(styles)(RouterComponent));
