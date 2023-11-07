import React, { Component } from 'react';
import Grid from '@mui/material/Grid';
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import StopIcon from '@mui/icons-material/Stop';
import { grey } from '@mui/material/colors';
import { styled } from '@mui/system';

export type ArrowProps = {
  active: boolean;
  rotate: number;
};

const Arrow = ({ active, rotate }: ArrowProps) => {
  const ActiveIcon = styled(ArrowUpwardIcon)(({ theme }) => ({
    backgroundColor: theme.palette.primary.main,
    borderRadius: '30px',
    color: grey[50],
  }));

  const Icon = active ? ActiveIcon : ArrowUpwardIcon;

  return (
    <Grid item xs={4} style={{ textAlign: 'center' }}>
      <Icon style={{ rotate: `${rotate}deg` }} />
    </Grid>
  );
};

export type IdleProps = {
  active: boolean;
};

const Idle = ({ active }: IdleProps) => {
  const ActiveIcon = styled(StopIcon)(({ theme }) => ({
    backgroundColor: theme.palette.primary.main,
    borderRadius: '30px',
    color: grey[50],
  }));

  const Icon = active ? ActiveIcon : ArrowUpwardIcon;

  return (
    <Grid item xs={4} style={{ textAlign: 'center' }}>
      <Icon />
    </Grid>
  );
};

export type DpadProps = {
  up: number;
  down: number;
  left: number;
  right: number;
};

export const Dpad = (props: DpadProps) => {
  const getActiveIndex = (): number => {
    if (props.up) {
      if (props.left) {
        return 1;
      } else if (props.right) {
        return 3;
      } else {
        return 2;
      }
    } else if (props.down) {
      if (props.left) {
        return 7;
      } else if (props.right) {
        return 9;
      } else {
        return 8;
      }
    } else {
      if (props.left) {
        return 4;
      } else if (props.right) {
        return 6;
      } else {
        return 5;
      }
    }
  };

  const activeIndex = getActiveIndex();

  return (
    <Grid container style={{ width: '83.75px', margin: 'auto' }}>
      <Arrow active={activeIndex === 1} rotate={-45} />
      <Arrow active={activeIndex === 2} rotate={0} />
      <Arrow active={activeIndex === 3} rotate={45} />
      <Arrow active={activeIndex === 4} rotate={-90} />
      <Idle active={activeIndex === 5} />
      <Arrow active={activeIndex === 6} rotate={90} />
      <Arrow active={activeIndex === 7} rotate={-135} />
      <Arrow active={activeIndex === 8} rotate={180} />
      <Arrow active={activeIndex === 9} rotate={135} />
    </Grid>
  );
};
