import React, { ReactNode, ReactElement } from 'react';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import { StyleGenerator, Styled } from '../../utils';
import { withStyles } from '@mui/styles';

type ClassNames = 'centered';

const styles: StyleGenerator<ClassNames> = () => ({
  centered: {
    textAlign: 'center',
  },
});

export type AsyncProps = {
  isLoading: boolean;
  isSuccess: boolean;
  error: unknown;
  children: () => ReactElement;
  size?: number | string;
};

type InternalAsyncProps = Styled<ClassNames> & AsyncProps;

const AsyncComponent = ({ classes, size, isLoading, isSuccess, error, children }: InternalAsyncProps) => {
  if (isLoading) {
    return (
      <div className={classes.centered}>
        <CircularProgress size={size || '10rem'} />
      </div>
    );
  }
  if (isSuccess) {
    return children();
  }

  return <Typography color="error">{error}</Typography>;
};

export const Async = withStyles(styles)(AsyncComponent);
