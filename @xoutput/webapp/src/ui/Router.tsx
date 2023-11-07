import React, { ReactElement } from 'react';
import { Route, Routes, useParams } from 'react-router';
import { Notifications } from './notifications/Notifications';
import { MainMenu } from './MainMenu';
import { InputReader } from './input/InputReader';
import { EmulatedControllers } from './emulation/EmulatedControllers';
import { MappedControllers } from './mapping/MappedControllers';
import { InputDevices } from './input/InputDevices';
import { InputDevice } from './input/InputDevice';

export type RouterProps = {};

type ParamType = string | Record<string, string | undefined>;

type ReadParamsProps<T extends ParamType> = {
  children: (data: T) => ReactElement;
};

const ReadParams = <T extends ParamType>({ children }: ReadParamsProps<T>) => {
  const params: any = useParams<T>();
  return children(params);
};

export const Router = ({ }: RouterProps) => {
  return (
    <>
      <Routes>
        <Route path="/emulation" />
        <Route path="*" element={<MainMenu />} />
      </Routes>
      <div style={{ margin: '8px' }}>
        <Routes>
          <Route path="/" element={<>asd</>} />
          <Route path="/inputs" element={<InputDevices />} />
          <Route path="/inputs/:id" element={<ReadParams>{({ id }: { id: string }) => <InputDevice id={id}></InputDevice>}</ReadParams>} />
          <Route path="/emulated/controllers" element={<EmulatedControllers />} />
          <Route path="/mapped/controllers" element={<MappedControllers />} />
          <Route path="/inputreader" element={<InputReader />} />
          <Route path="/notifications" element={<Notifications />} />
        </Routes>
      </div>
    </>
  );
};
