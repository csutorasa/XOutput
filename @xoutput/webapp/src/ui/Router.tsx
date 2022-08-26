import React, { ReactElement } from 'react';
import { Route, Routes, useParams } from 'react-router';
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

type InternalRouterProps = Styled<ClassNames> & RouterProps;

type ParamType = string | Record<string, string | undefined>;

type ReadParamsProps<T extends ParamType> = {
  children: (data: T) => ReactElement;
};

const ReadParams = <T extends ParamType>({ children }: ReadParamsProps<T>) => {
  const params: any = useParams<T>();
  return children(params);
};

const RouterComponent = ({ classes }: InternalRouterProps) => {
  return (
    <>
      <Routes>
        <Route path="/emulation" />
        <Route>
          <MainMenu />
        </Route>
      </Routes>
      <div className={classes.mainContent}>
        <Routes>
          <Route path="/">
            <div></div>
          </Route>
          <Route path="/inputs">
            <InputDevices></InputDevices>
          </Route>
          <Route path="/inputs/:id">
            <ReadParams>{({ id }: { id: string }) => <InputDevice id={id}></InputDevice>}</ReadParams>
          </Route>
          <Route path="/emulated/controllers">
            <EmulatedControllers></EmulatedControllers>
          </Route>
          <Route path="/mapped/controllers">
            <MappedControllers></MappedControllers>
          </Route>
          <Route path="/inputreader">
            <InputReader></InputReader>
          </Route>
          <Route path="/notifications">
            <Notifications></Notifications>
          </Route>
        </Routes>
      </div>
    </>
  );
};

export const Router = withStyles(styles)(RouterComponent);
