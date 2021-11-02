import Typography from '@mui/material/Typography';
import React, { useState } from 'react';
import { GamepadReader } from '../../gamepad/GamepadReader';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import { GamepadValue } from './GamepadValue';

export type GamepadProps = {
  gamepad: GamepadReader;
};

const GamepadComponent = ({ gamepad }: GamepadProps) => {
  const [running, setRunning] = useState(gamepad.isRunning());

  const setGamepadState = () => {
    const running = gamepad.isRunning();
    if (running) {
      gamepad.stop();
      setRunning(false);
    } else {
      gamepad.start().then(
        () => {
          setRunning(true);
        },
        () => {
          /*error handling*/
        }
      );
    }
  };

  return (
    <Card>
      <CardContent>
        <Typography variant="h4">
          {gamepad.gamepad.id} ({gamepad.gamepad.index})
        </Typography>
        {gamepad.gamepad.axes.length} axes {gamepad.gamepad.buttons.length} buttons
        <div style={{ display: 'flex' }}>
          {gamepad.gamepad.axes.map((_, i) => (
            <GamepadValue key={i} index={i} type="axis" valueGetter={() => gamepad.gamepad.axes[i]} />
          ))}
        </div>
        <div style={{ display: 'flex' }}>
          {gamepad.gamepad.buttons.map((_, i) => (
            <GamepadValue key={i} index={i} type="button" valueGetter={() => gamepad.gamepad.buttons[i].value} />
          ))}
        </div>
      </CardContent>
      <CardActions>
        <Button onClick={() => setGamepadState()}>{running ? 'Stop' : 'Start'}</Button>
      </CardActions>
    </Card>
  );
};

export const Gamepad = GamepadComponent;
