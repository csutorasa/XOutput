import './index.scss';

import { render } from 'react-dom';
import { RootElement } from './ui/RootElement';
import { WebSocketService } from './communication/websocket';
import { http } from './communication/http';
import 'typeface-roboto';

const host = window.location.hostname;
const port = window.location.port;
http.initialize(host, port);
WebSocketService.initialize(host, port);
// const websocket = new WebSocketService();

window.onerror = (msg, url, line, col) => {
  /* if (websocket.isReady()) {
        websocket.sendDebug(`Error in ${url} at ${line}:${col} ${msg}`);
    } */
};

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
