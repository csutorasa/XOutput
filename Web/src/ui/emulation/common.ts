import { EventHolder } from "../../events/eventholder";
import { WebSocketService } from "../../communication/Websocket";

export interface CommonProps {
    eventHolder: EventHolder;
    websocket: WebSocketService;
}