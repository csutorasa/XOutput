import { UIInputFlow } from "./base";
import { MouseEvent, Touch } from "react";

export class EventHolder {
    private mouseFlow: UIInputFlow;
    private touchFlows: { [key: number]: UIInputFlow } = {};

    mouseAdd(flow: UIInputFlow, event: MouseEvent) {
        if (this.mouseFlow) {
            this.mouseFlow.end();
        }
        this.mouseFlow = flow;
        flow.start(event);
    }

    mouseMove(event: MouseEvent) {
        if (this.mouseFlow) {
            this.mouseFlow.move(event);
        }
    }

    mouseEnd() {
        if (this.mouseFlow) {
            this.mouseFlow.end();
            this.mouseFlow = null;
        }
    }

    touchAdd(flow: UIInputFlow, event: Touch) {
        const identifier: number = event.identifier;
        this.touchFlows[identifier] = flow;
        flow.start(event);
    }

    touchMove(event: Touch) {
        const identifier: number = event.identifier;
        if (identifier in this.touchFlows) {
            const flow = this.touchFlows[identifier];
            flow.move(event);
        }
    }

    touchEnd(event: Touch) {
        const identifier: number = event.identifier;
        const flow = this.touchFlows[identifier];
        if (flow) {
            flow.end();
            delete this.touchFlows[identifier];
        }
    }

    touchEndAll() {
        for (let key in Object.keys(this.touchFlows)) {
            const flow = this.touchFlows[key];
            flow.end();
            delete this.touchFlows[key];
        }
    }
}


