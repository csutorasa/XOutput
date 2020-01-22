import React from "react";
import { Slider } from "./slider";
import { Button } from "./button";
import { Axis } from "./axis";

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
                </table>
            </div>
            <div style={{ flexGrow: 7 }}>
                <table>
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