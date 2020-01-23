import React, { MouseEvent, ReactElement } from "react";
import { Slider } from "./slider";
import { Button } from "./button";
import { Axis } from "./axis";
import { ListEmulatorsResponse, Rest, EmulatorResponse } from "../communication/rest";
import { Websocket } from "../communication/websocket";

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
            return 'Connected';
        }
        return 'INVALID STATE';
        /*return <>
            <Button input="L1" flexGrow={2}></Button>
            <Axis input="L" flexGrow={3}></Axis>
            <Axis input="R" flexGrow={3}></Axis>
            <Button input="R1" flexGrow={2}></Button>
        </>;*/
    }
}

export const RootElement = <>
    <MainMenu></MainMenu>
</>;

class FirstLine extends React.Component {
    render() {
        return <>
            <Button input="L2" flexGrow={2}></Button>
            <Slider input="L2" flexGrow={3}></Slider>
            <Slider input="R2" flexGrow={3}></Slider>
            <Button input="R2" flexGrow={2}></Button>
        </>;
    }
}

class SecondLine extends React.Component {
    render() {
        return <>
            <div className="dpad" style={{ flexGrow: 6, width: "1%" }}>
                <div className="fill"></div>
                <div className="text">DPad</div>
            </div>
            <div style={{ flexGrow: 7 }}>
                <table>
                    <tbody>
                        <tr>
                            <td className="button circle Back">Back</td>
                            <td className="fullscreen">Fullscreen</td>
                            <td className="button circle Start">Start</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td className="button circle Home">Home</td>
                            <td></td>
                        </tr>
                        <tr>
                            <td className="button circle L3">L3</td>
                            <td></td>
                            <td className="button circle R3">R3</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div style={{ flexGrow: 7 }}>
                <table>
                    <tbody>
                        <tr>
                            <td></td>
                            <td className="button circle Y" style={{ backgroundColor: "yellow", color: "orangered" }}>Y</td>
                            <td></td>
                        </tr>
                        <tr>
                            <td className="button circle X" style={{ backgroundColor: "blue", color: "lightblue" }}>X</td>
                            <td></td>
                            <td className="button circle B" style={{ backgroundColor: "red", color: "darkred" }}>B</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td className="button circle A" style={{ backgroundColor: "green", color: "lightgreen" }}>A</td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </>;
    }
}

class ThirdLine extends React.Component {
    render() {
        return <>
            <Button input="L1" flexGrow={2}></Button>
            <Axis input="L" flexGrow={3}></Axis>
            <Axis input="R" flexGrow={3}></Axis>
            <Button input="R1" flexGrow={2}></Button>
        </>;
    }
}


export const Hello = <div className="root">
    {/* <div style={{ width: 0, height: "25%" }}></div> */}
    <FirstLine></FirstLine>

    <div style={{ flexBasis: "100%", height: 0 }}></div>

    {/* <div style={{ width: 0, height: "40%" }}></div> */}
    <SecondLine></SecondLine>

    <div style={{ flexBasis: "100%", height: 0 }}></div>

    {/* <div style={{ width: 0, height: "35%" }}></div> */}
    <ThirdLine></ThirdLine>
</div>;