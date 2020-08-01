import { EventHolder } from "../../events/eventholder";
import { WebSocketService } from "../../communication/websocket";

export interface CommonProps {
    eventHolder: EventHolder;
    websocket: WebSocketService;
}