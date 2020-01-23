import './index.scss'

import { render } from 'react-dom';
import { RootElement } from './ui/main';
import { Websocket } from './communication/websocket';
import { UIInputFlow } from './events/base';
import { EventHolder } from './events/eventholder';
import { ButtonFlow } from './events/button';
import { AxisFlow } from './events/axis';
import { SliderFlow } from './events/slider';
import { DPadFlow } from './events/dpad';
import { Http } from './communication/http'; 

const host = window.location.hostname;
const port = window.location.port;
Http.initialize(host, port);
Websocket.initialize(host, port);

window.onerror = function(msg, url, line, col) {
    if (Websocket.isReady()) {
        Websocket.sendDebug("Error in " + url + " at " + line + ":" + col + " " + msg);
    }
};

const eventHolder = new EventHolder();

const element = document.createElement('div');
document.body.appendChild(element);
render(RootElement, element);


function openFullscreen() {
    document.querySelector(".root").requestFullscreen().catch((err) => {
        Websocket.sendDebug(err);
    });
}
/*
function handleTouchEvent(event: TouchEvent, action: (t: Touch) => void) {
    Array.from(event.changedTouches).forEach(t => {
        action(t);
    });
    event.preventDefault();
}

function registerStartEvents(className: string, flowCreator: (communication: Communication, element: HTMLElement) => UIInputFlow) {
    Array.from(document.getElementsByClassName(className)).forEach(e => {
        const element = <HTMLElement>e;
        element.addEventListener('mousedown', (event: Event) => {
            const flow = flowCreator(communication, element);
            eventHolder.mouseAdd(flow, event as MouseEvent);
        });
        element.addEventListener('touchstart', (event: TouchEvent) =>  handleTouchEvent(event, (touch) => {
            const flow = flowCreator(communication, element);
            eventHolder.touchAdd(flow, touch);
        }));
    });
}

registerStartEvents('button', (communication: Communication, element: HTMLElement) => new ButtonFlow(communication, element));
registerStartEvents('slider', (communication: Communication, element: HTMLElement) => new SliderFlow(communication, element));
registerStartEvents('twodimensional', (communication: Communication, element: HTMLElement) => new AxisFlow(communication, element));
registerStartEvents('dpad', (communication: Communication, element: HTMLElement) => new DPadFlow(communication, element));

document.addEventListener('mousemove', (event) => eventHolder.mouseMove(event));
document.addEventListener('touchmove', (event) => handleTouchEvent(event, (t) => eventHolder.touchMove(t)));

document.addEventListener('mouseup', () => eventHolder.mouseEnd());
document.addEventListener('touchend', (event: TouchEvent) => handleTouchEvent(event, (t) => eventHolder.touchEnd(t)));
document.addEventListener('touchcancel', () => eventHolder.touchEndAll());

const fullscreen = document.querySelector('.fullscreen');
fullscreen.addEventListener('click', () => openFullscreen());
fullscreen.addEventListener('touchstart', () => openFullscreen());
*/