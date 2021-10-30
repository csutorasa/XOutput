import React from 'react';
import { Switch, Route, Redirect, RouteChildrenProps, withRouter, RouteComponentProps } from 'react-router';
import { XboxEmulation } from './emulation/xbox';
import withStyles from '@mui/styles/withStyles';
import { Ds4Emulation } from './emulation/ds4';
import { Styled, StyleGenerator } from '../utils';
import { Notifications } from './notifications/Notifications';
import { MainMenu } from './MainMenu';
import { InputReader } from './input/InputReader';

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
          <Route path="/controllers">
            <div></div>
          </Route>
          <Route path="/devices">
            <div></div>
          </Route>
          <Route
            path="/emulation/MicrosoftXbox360/:emulator"
            component={(props: RouteChildrenProps<{ emulator: string }>) => (
              <XboxEmulation deviceType="MicrosoftXbox360" emulator={props.match.params.emulator}></XboxEmulation>
            )}
          />
          <Route
            path="/emulation/SonyDualShock4/:emulator"
            component={(props: RouteChildrenProps<{ emulator: string }>) => (
              <Ds4Emulation deviceType="SonyDualShock4" emulator={props.match.params.emulator}></Ds4Emulation>
            )}
          />
          <Route path="/inputs" exact>
            <div></div>
          </Route>
          <Route path="/inputs/:id" component={(props: RouteChildrenProps<{ id: string }>) => <div id={props.match.params.id}></div>} />
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
