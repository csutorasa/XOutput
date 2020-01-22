import React from "react";

export interface AxisProp {
    input: string;
    flexGrow: number;
}

export class Axis extends React.Component<AxisProp> {

    constructor(props: Readonly<AxisProp>) {
        super(props);
    }

    render() {
        return <div className="twodimensional" style={{ flexGrow: this.props.flexGrow }}>
            <div className="fill"></div>
            <div className="text">{this.props.input}</div>
        </div>;
    }
}