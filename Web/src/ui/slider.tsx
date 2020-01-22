import React from "react";

export interface SliderProp {
    input: string;
    flexGrow: number;
}

export class Slider extends React.Component<SliderProp> {

    constructor(props: Readonly<SliderProp>) {
        super(props);
    }

    render() {
        return <div className="slider" style={{ flexGrow: this.props.flexGrow }}>
            <div className="fill"></div>
            <div className="text">{this.props.input}</div>
        </div>;
    }
}