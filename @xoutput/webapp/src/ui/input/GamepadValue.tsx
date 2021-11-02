import React, { useEffect, useState } from 'react';

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
