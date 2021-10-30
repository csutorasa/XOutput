import React, { CSSProperties } from 'react';

export interface SquareProp {
  style: CSSProperties;
}

export class Square extends React.Component<SquareProp> {
  constructor(props: Readonly<SquareProp>) {
    super(props);
  }

  render() {
    return (
      <div className="square" style={this.props.style}>
        {this.props.children}
      </div>
    );
  }
}
