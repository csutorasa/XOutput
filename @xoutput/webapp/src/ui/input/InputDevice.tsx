import Typography from '@mui/material/Typography';
import Card from '@mui/material/Card';
import { inputDeviceFeedbackClient, WebSocketSession } from '@xoutput/client';
import React, { useEffect } from 'react';
import { useInputQuery } from '../../queries/useInputsQuery';
import { Async } from '../components/Asnyc';
import { GamepadValue } from './GamepadValue';

export type InputDeviceProps = {
  id: string;
};

const InputDeviceComponent = ({ id }: InputDeviceProps) => {
  const { data: input, isLoading, isSuccess, error } = useInputQuery(id);

  const data: Record<number, number | undefined> = {};

  useEffect(() => {
    let session: WebSocketSession;
    inputDeviceFeedbackClient
      .connect(id, (feedback) => {
        feedback.sources.forEach((s) => {
          data[s.id] = s.value;
        });
      })
      .then((s) => {
        session = s;
      });
    return () => {
      if (session) {
        session.close();
      }
    };
  });

  return (
    <Async isLoading={isLoading} isSuccess={isSuccess} error={error}>
      {() => (
        <>
          <Card>
            <Typography variant="h4">
              {input.name} ({input.id})
            </Typography>
            <div>{input.deviceApi}</div>
            {input.sources.filter((s) => s.type.startsWith('Axis')).length} axes {input.sources.filter((s) => s.type === 'Button').length}{' '}
            buttons {input.sources.filter((s) => s.type === 'Slider').length} sliders{' '}
            {input.sources.filter((s) => s.type === 'Dpad').length} dpads
            <div style={{ display: 'flex' }}>
              {input.sources
                .filter((s) => s.type.startsWith('Axis'))
                .map((s, i) => (
                  <GamepadValue key={i} index={i} type="axis" valueGetter={() => data[s.id] ?? 0} />
                ))}
            </div>
            <div style={{ display: 'flex' }}>
              {input.sources
                .filter((s) => s.type === 'Button')
                .map((s, i) => (
                  <GamepadValue key={i} index={i} type="button" valueGetter={() => data[s.id] ?? 0} />
                ))}
            </div>
            <div style={{ display: 'flex' }}>
              {input.sources
                .filter((s) => s.type === 'Slider')
                .map((s, i) => (
                  <GamepadValue key={i} index={i} type="slider" valueGetter={() => data[s.id] ?? 0} />
                ))}
            </div>
            <div style={{ display: 'flex' }}>
              {input.sources
                .filter((s) => s.type === 'Dpad')
                .map((s, i) => (
                  <GamepadValue key={i} index={i} type="dpad" valueGetter={() => data[s.id] ?? 0} />
                ))}
            </div>
          </Card>
        </>
      )}
    </Async>
  );
};

export const InputDevice = InputDeviceComponent;
