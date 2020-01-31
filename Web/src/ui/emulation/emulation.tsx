import React, { MouseEvent, ReactElement } from "react";
import { Dpad } from "./dpad";
import { Slider } from "./slider";
import { Button } from "./button";
import { Axis } from "./axis";
import { ListEmulatorsResponse, rest, EmulatorResponse } from "../../communication/rest";
import { websocket } from "../../communication/websocket";
import { Square } from "./square";

interface EmulationState {
    emulators: ListEmulatorsResponse;
    loading: boolean;
}

export interface EmulationProps {
    deviceType: string;
    emulator: string;
}

export class Emulation extends React.Component<EmulationProps, EmulationState, any> {

    constructor(props: Readonly<any>) {
        super(props);
        this.state = {
            emulators: null,
            loading: true,
        }
    }

    componentDidMount() {
        websocket.connect(`${this.props.deviceType}/${this.props.emulator}`).then(() => {
            this.setState((state, props) => {
                return {
                    emulators: state.emulators,
                    loading: false,
                };
            });
        });
    }

    componentWillUnmount() {
        websocket.close();
    }

    render() {
        if (this.state.loading) {
            return <h1>Loading</h1>;
        }
        return <div className="root">
            <Button input="L2" style={{ gridColumn: "span 4", gridRow: "span 5" }}></Button>
            <Slider input="L2" style={{ gridColumn: "span 6", gridRow: "span 5" }}></Slider>
            <Slider input="R2" style={{ gridColumn: "span 6", gridRow: "span 5" }}></Slider>
            <Button input="R2" style={{ gridColumn: "span 4", gridRow: "span 5" }}></Button>

            <Dpad style={{ gridColumn: "span 7", gridRow: "span 8" }}></Dpad>
            <Square style={{ gridColumn: "span 7", gridRow: "span 8" }}>
                <div><Button input="Back" circle={true}></Button></div>
                <div className="fullscreen">Fullscreen</div>
                <div><Button input="Start" circle={true}></Button></div>
                <div></div>
                <div><Button input="Home" circle={true}></Button></div>
                <div></div>
                <div><Button input="L3" circle={true}></Button></div>
                <div></div>
                <div><Button input="R3" circle={true}></Button></div>
            </Square>
            <Square style={{ gridColumn: "span 6", gridRow: "span 8" }}>
                <div></div>
                <div><Button input="Y" circle={true} style={{ backgroundColor: "yellow", color: "orangered" }}></Button></div>
                <div></div>
                <div><Button input="X" circle={true} style={{ backgroundColor: "blue", color: "lightblue" }}></Button></div>
                <div></div>
                <div><Button input="B" circle={true} style={{ backgroundColor: "red", color: "darkred" }}></Button></div>
                <div></div>
                <div><Button input="A" circle={true} style={{ backgroundColor: "green", color: "lightgreen" }}></Button></div>
                <div></div>
            </Square>

            <Button input="L1" style={{ gridColumn: "span 4", gridRow: "span 7" }}></Button>
            <Axis input="L" style={{ gridColumn: "span 6", gridRow: "span 7" }}></Axis>
            <Axis input="R" style={{ gridColumn: "span 6", gridRow: "span 7" }}></Axis>
            <Button input="R1" style={{ gridColumn: "span 4", gridRow: "span 7" }}></Button>
        </div>;
    }
}
