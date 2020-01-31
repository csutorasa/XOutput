import './index.scss'

import { render } from 'react-dom';
import { RootElement } from './ui/main';
import { WebSocketService } from './communication/websocket';
import { http } from './communication/http'; 

const host = window.location.hostname;
const port = window.location.port;
http.initialize(host, port);
WebSocketService.initialize(host, port);
const websocket = new WebSocketService();
//websocket.connect("");

window.onerror = function(msg, url, line, col) {
    if (websocket.isReady()) {
        websocket.sendDebug(`Error in ${url} at ${line}:${col} ${msg}`);
    }
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
