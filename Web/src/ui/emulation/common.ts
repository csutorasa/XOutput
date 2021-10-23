import { EventHolder } from '../../events/eventholder';
import { WebSocketService, WebSocketSession } from '../../client/websocket/websocket';

export interface CommonProps {
  eventHolder: EventHolder;
  websocket: WebSocketSession;
}
