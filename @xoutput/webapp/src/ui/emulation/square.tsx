import React, { CSSProperties, ReactNode } from 'react';

export type SquareProp = {
  style: CSSProperties;
  children: ReactNode;
}

export const Square = ({ style, children }: SquareProp) => {
  return (
    <div className="square" style={style}>
      {children}
    </div>
  );
}
