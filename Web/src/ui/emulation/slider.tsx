import React, { CSSProperties } from "react";

export interface SliderProp {
    input: string;
    style: CSSProperties;
}

export class Slider extends React.Component<SliderProp> {

    constructor(props: Readonly<SliderProp>) {
        super(props);
    }

    render() {
        return <div className="slider" style={this.props.style}>
            <div className="fill"></div>
            <div className="text">
                <div className="inner">{this.props.input}</div>
            </div>
        </div>;
    }
}