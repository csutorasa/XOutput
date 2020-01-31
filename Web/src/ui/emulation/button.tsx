import React, { CSSProperties } from "react";

export interface ButtonProp {
    input: string;
    style?: CSSProperties;
    circle?: boolean;
}

export class Button extends React.Component<ButtonProp> {

    constructor(props: Readonly<ButtonProp>) {
        super(props);
    }

    render() {
        return <div className={this.props.circle ? "button circle" : "button"} style={this.props.style ? this.props.style : {}}>
            <div className="inner">
                {this.props.input}
            </div>
        </div>;
    }
}