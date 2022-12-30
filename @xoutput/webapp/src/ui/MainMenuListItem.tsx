import React, { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import ListItemButton from '@mui/material/ListItemButton';

export type MainMenuListItemProps = {
  onClick: () => void;
  path: string;
  children: ReactNode;
};

const MainMenuListItemComponent = ({ path, onClick, children }: MainMenuListItemProps) => {
  return (
    <ListItemButton component={Link} to={path} onClick={() => (onClick ? onClick() : null)} selected={path === location.pathname}>
      {children}
    </ListItemButton>
  );
};

export const MainMenuListItem = MainMenuListItemComponent;
