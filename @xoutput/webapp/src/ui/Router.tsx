import React, { ReactElement } from 'react';
import { Switch, Route, Redirect, withRouter, RouteComponentProps, useParams } from 'react-router';
import withStyles from '@mui/styles/withStyles';
import { Styled, StyleGenerator } from '../utils';
import { Notifications } from './notifications/Notifications';
import { MainMenu } from './MainMenu';
import { InputReader } from './input/InputReader';
import { EmulatedControllers } from './emulation/EmulatedControllers';
import { MappedControllers } from './mapping/MappedControllers';
import { InputDevices } from './input/InputDevices';
import { InputDevice } from './input/InputDevice';

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

type ReadParamsProps<T> = {
  children: (data: T) => ReactElement;
};

const ReadParams = <T,>({ children }: ReadParamsProps<T>) => {
  const params = useParams<T>();
  return children(params);
};

const RouterComponent = ({ classes }: InternalRouterProps) => {
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
          <Route path="/inputs" exact>
            <InputDevices></InputDevices>
          </Route>
          <Route path="/inputs/:id">
            <ReadParams>{({ id }: { id: string }) => <InputDevice id={id}></InputDevice>}</ReadParams>
          </Route>
          <Route path="/emulated/controllers" exact>
            <EmulatedControllers></EmulatedControllers>
          </Route>
          <Route path="/mapped/controllers" exact>
            <MappedControllers></MappedControllers>
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
