import Typography from '@mui/material/Typography';
import React, { useCallback, useEffect, useState } from 'react';
import { GamepadReader } from '../../gamepad/GamepadReader';
import Button from '@mui/material/Button';
import { Card, CardActions, CardContent } from '@mui/material';
import OpenWithIcon from '@mui/icons-material/OpenWith';
import RadioButtonCheckedIcon from '@mui/icons-material/RadioButtonChecked';

export type GamepadValueProps = {
  index: number;
  type: 'button' | 'axis';
  valueGetter: () => number;
};

const GamepadValueComponent = ({ index, type, valueGetter }: GamepadValueProps) => {
  const [value, setValue] = useState(0);

  useEffect(() => {
    const intervalId = setInterval(() => {
      setValue(valueGetter());
    }, 10);
    return () => {
      clearInterval(intervalId);
    };
  }, []);

  return (
    <div style={{ padding: '3px', width: '32px' }}>
      {type === 'button' ? 'B' : 'A'}
      {index}
      <br /> {value.toFixed(2)}
    </div>
  );
};

export const GamepadValue = GamepadValueComponent;
