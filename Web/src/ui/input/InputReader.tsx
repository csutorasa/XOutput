import Typography from '@mui/material/Typography';
import Alert from '@mui/material/Alert';
import React, { useEffect, useState } from 'react';
import { gamepadService } from '../../gamepad/GamepadService';
import Translation from '../../translation/Translation';
import { GamepadReader } from '../../gamepad/GamepadReader';
import Button from '@mui/material/Button';

export type InputReaderProps = {};

const InputReaderComponent = ({}: InputReaderProps) => {
  if (!gamepadService.isAvailable()) {
    return (
      <>
        The browser is not compatible with the Gampad API.
        <br />
        Please check compatiblity{' '}
        <a href="https://developer.mozilla.org/en-US/docs/Web/API/Gamepad_API/Using_the_Gamepad_API#browser_compatibility">here</a>.
      </>
    );
  }
  const [gamepads, setGamepads] = useState(gamepadService.getGamepads());

  useEffect(() => {
    const listener = (gamepads: GamepadReader[]) => {
      setGamepads(gamepads);
    };
    gamepadService.addChangeListener(listener);
    return () => {
      gamepadService.removeChangeListener(listener);
    };
  });

  return (
    <>
      <Typography variant="h3">{Translation.translate('InputReader')}</Typography>
      <Alert severity="info">If you cannot see your controller, press a button or move an axis.</Alert>
      {gamepads.map((gamepad) => (
        <div key={gamepad.gamepad.id}>
          {gamepad.gamepad.id} <Button onClick={() => gamepad.start()}>Start</Button>
        </div>
      ))}
    </>
  );
};

export const InputReader = InputReaderComponent;
