import React, { ReactElement } from 'react';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';

export type AsyncProps = {
  isLoading: boolean;
  isSuccess: boolean;
  error: unknown;
  children: () => ReactElement;
  size?: number | string;
};

export const Async = ({ size, isLoading, isSuccess, error, children }: AsyncProps) => {
  if (isLoading) {
    return (
      <div style={{ textAlign: 'center' }}>
        <CircularProgress size={size || '10rem'} />
      </div>
    );
  }
  if (isSuccess) {
    return children();
  }

  return <Typography color="error">{error?.toString()}</Typography>;
};
