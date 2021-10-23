import { EventHolder } from '../../events/eventholder';
import { WebSocketService, WebSocketSession } from '../../client/websocket';

export interface CommonProps {
  eventHolder: EventHolder;
  websocket: WebSocketSession;
}
