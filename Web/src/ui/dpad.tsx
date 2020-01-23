import React, { CSSProperties } from "react";

export interface DpadProp {
    style: CSSProperties;
}

export class Dpad extends React.Component<DpadProp> {

    constructor(props: Readonly<DpadProp>) {
        super(props);
    }

    render() {
        return <div className="dpad" style={this.props.style}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">DPad</div>
            </div>
        </div>;
    }
}