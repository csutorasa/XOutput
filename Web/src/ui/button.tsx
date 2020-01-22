import React from "react";

export interface ButtonProp {
    input: string;
    flexGrow: number;
}

export class Button extends React.Component<ButtonProp> {

    constructor(props: Readonly<ButtonProp>) {
        super(props);
    }

    render() {
        return <div className="button" style={{ flexGrow: this.props.flexGrow }}>
        {this.props.input}
    </div>;
    }
}