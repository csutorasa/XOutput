import { EventHolder } from "../../events/eventholder";
import { WebSocketService, WebSocketSession } from "../../communication/websocket";

export interface CommonProps {
    eventHolder: EventHolder;
    websocket: WebSocketSession;
}