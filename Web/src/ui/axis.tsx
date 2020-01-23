import React, { CSSProperties } from "react";

export interface AxisProp {
    input: string;
    style: CSSProperties;
}

export class Axis extends React.Component<AxisProp> {

    constructor(props: Readonly<AxisProp>) {
        super(props);
    }

    render() {
        return <div className="twodimensional" style={this.props.style}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">{this.props.input}</div>
            </div>
        </div>;
    }
}