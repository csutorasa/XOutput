import React, { MouseEvent, ReactElement } from "react";
import { Dpad } from "./dpad";
import { Slider } from "./slider";
import { Button } from "./button";
import { Axis } from "./axis";
import { ListEmulatorsResponse, Rest, EmulatorResponse } from "../communication/rest";
import { Websocket } from "../communication/websocket";
import { Square } from "./square";

enum MainMenuStatus {
    New,
    Initialized,
    Connecting,
    Connected
}

interface MainMenuState {
    emulators: ListEmulatorsResponse;
    status: MainMenuStatus;
}

export class MainMenu extends React.Component<any, MainMenuState, any> {

    constructor(props: Readonly<any>) {
        super(props);
        this.state = {
            emulators: null,
            status: MainMenuStatus.New,
        }
        Rest.getEmulators().then(r => {
            this.setState((state, props) => {
                return {
                    emulators: r,
                    status: MainMenuStatus.Initialized,
                };
            });
        });
    }

    private onSelection(event: MouseEvent, emulator: string, deviceType: string) {
        this.setState((state, props) => {
            return {
                emulators: state.emulators,
                status: MainMenuStatus.Connecting,
            };
        });
        Websocket.connect(`${deviceType}/${emulator}`).then(() => {
            this.setState((state, props) => {
                return {
                    emulators: state.emulators,
                    status: MainMenuStatus.Connected,
                };
            });
        });
    }

    private renderListElement(key: string, value: EmulatorResponse): ReactElement {
        return <div key={key}>
            <h3>{key}</h3>
            <p>
                {value.SupportedDeviceTypes.map(t => <button key={key} disabled={!value.Installed} onClick={(e) => this.onSelection(e, key, t)}>{t}</button>)}
            </p>
        </div>;
    }

    render() {
        if (this.state.status == MainMenuStatus.New) {
            return <h1>Loading</h1>;
        }
        if (this.state.status == MainMenuStatus.Initialized) {
            return <>
                {Object.keys(this.state.emulators).map(key => this.renderListElement(key, this.state.emulators[key]))}
            </>;
        }
        if (this.state.status == MainMenuStatus.Connecting) {
            return <h1>Loading</h1>;
        }
        if (this.state.status == MainMenuStatus.Connected) {
            return <div className="root">
                <Button input="L2" style={{gridColumn: "span 4", gridRow: "span 5"}}></Button>
                <Slider input="L2" style={{gridColumn: "span 6", gridRow: "span 5"}}></Slider>
                <Slider input="R2" style={{gridColumn: "span 6", gridRow: "span 5"}}></Slider>
                <Button input="R2" style={{gridColumn: "span 4", gridRow: "span 5"}}></Button>

                <Dpad style={{gridColumn: "span 7", gridRow: "span 8"}}></Dpad>
                <Square style={{gridColumn: "span 7", gridRow: "span 8"}}>
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
                <Square style={{gridColumn: "span 6", gridRow: "span 8"}}>
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

                <Button input="L1" style={{gridColumn: "span 4", gridRow: "span 7"}}></Button>
                <Axis input="L" style={{gridColumn: "span 6", gridRow: "span 7"}}></Axis>
                <Axis input="R" style={{gridColumn: "span 6", gridRow: "span 7"}}></Axis>
                <Button input="R1" style={{gridColumn: "span 4", gridRow: "span 7"}}></Button>
            </div>;
        }
        return 'INVALID STATE';
    }
}

export const RootElement = <>
    <MainMenu></MainMenu>
</>;
