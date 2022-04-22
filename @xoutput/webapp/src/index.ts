import './index.scss';

import { render } from 'react-dom';
import { RootElement } from './ui/RootElement';
import { gamepadService } from './gamepad/GamepadService';
import 'typeface-roboto';

gamepadService.start();

function disableResize() {
  const meta = document.createElement('meta');
  meta.name = 'viewport';
  meta.content = 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no';
  document.getElementsByTagName('head')[0].appendChild(meta);
}

disableResize();
const element = document.createElement('div');
document.body.appendChild(element);
render(RootElement, element);
