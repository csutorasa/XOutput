import { EventHolder } from '../../events/eventholder';
import { WebSocketSession } from '@xoutput/client';

export interface CommonProps {
  eventHolder: EventHolder;
  websocket: WebSocketSession;
}
